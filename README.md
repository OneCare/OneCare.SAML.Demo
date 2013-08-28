OneCare.SAML.Demo
=================

Demo app that shows how OneCare can SSO with partner applications using SAML


This demo app attempts to do the following:
-------------------------------------------
First, there are 3 players:

1. Partner IdP (full name is OneCare.Partner.IdP -- also referred to as just IdP) -- our partner's indentity provider
2. Partner SP (full name is OneCare.Partner.SP) -- our partner's service provider application
3. OneCare SP (full name is OneCare.Portal.SP) -- our service provider application

Now, here's the full sequence:

01. Launch Partner SP site (you're are not logged in)
02. Click the Login button -- you are redirected via SAML to the IdP (Partner IdP) site
03. There is no user in the auth cookie on the IdP site so it redirects to the Login page
04. You login as one of the IdP users and that user is saved in the IdP's Auth cookie
05. Via SAML the IdP user is sent back to the Partner SP
06. Because the Partner IdP and SP are partners, they've pre-associated users and the SP is able to look up 
it's user using the IdP's user info
07. The SP looks up it's user, stores that user in it's Auth cookie and presents the user with the option to 
connect to OneCare (the link only shows up when there's a user logged in)
08. You click on the OneCare link which causes a call to http://localhost/OneCare.Portal.SP/PartnerSSO/Login 
and, using basic auth, the SP's app name and password are sent in the headers
09. We are now in the PartnerSSO.Login controller method which extracts the Partner App name and password from 
the basic auth headers.  It then uses that information to look in the database (Raven) for a matching 
ExternalAppDoc.  It finds one and uses that document to get a Url for the IdP associated with that application 
(in other words we now know to use SAML to get a user from the Partner IdP).
10. The OneCare SP now makes a call through SAML to the Partner IdP
11. The Partner IdP sees that there is a user already logged in because it's in the Auth cookie and thus sends 
that user back to the OneCare SP
12. The OneCare SP tries to find a OneCare user that matches the partner app name and IdP username.  It can't 
find one so it redirects to it's own login page that explains to the user that such-and-such application is 
trying to associate itself with your OneCare account
13. You login and OneCare SP now updates the user record so that the next time it will be able to find the 
user from the partner app name and IdP username.  It also logs the user in and stores it's user in it's Auth 
cookie.


From the standpoint of the Controller's and their methods, the sequence is:
---------------------------------------------------------------------------
Click Login Link on OneCare.Partner.SP site
--> OneCare.Partner.SP.AccountController.Login (send's SAML Auth Request)
--> OneCare.Partner.IdP.SAMLController.SSOService (receive's SAML Auth Request)
--> OneCare.Partner.IdP.AccountController.Login (when there isn't a user already logged in, redirects back to 
SSOService)
--> OneCare.Partner.IdP.SAMLController.SSOService (we are sent back here after the user logs in, sends SAML 
Auth Response)
--> OneCare.Partner.SP.SAMLController.AssertionConsumerService (receive's SAML Auth Response, logs user in)
--> OneCare.Partner.SP.HomeController.Index (displays the user login status, including a new "Go To OneCare" link)

Click "Go To OneCare" Link on OneCare.Partner.SP site
--> OneCare.Partner.SP.OneCareController.SSOLogin (sends partner app creds using BA)
--> OneCare.Portal.SP.PartnerSSOController.Login (reads BA and looks up app's IdP then sends SAML Auth Request)
--> OneCare.Partner.IdP.SAMLController.SSOService (receive's SAML Auth Request, user already logged in, sends SAML
Auth Response)
--> OneCare.Portal.SP.SAMLController.AssertionConsumerService (receive's SAML Auth Response, looks for associated 
OneCare user)
--> OneCare.Portal.SP.PartnerSSOController.Associate (GET) (when an associated user can not be found)
--> OneCare.Portal.SP.PartnerSSOController.Associate (POST) (makes user association, redirects back to ACS)
--> OneCare.Portal.SP.SAMLController.AssertionConsumerService (we are sent back here after the user has been 
associated)
--> OneCare.Portal.SP.HomeController.Index (displays the user login status)


The SAML IdP and SP implementation was done using ComponentSpace's SAML component.  Read about that here:
---------------------------------------------------------------------------------------------------------
http://www.componentspace.com/Products/SAMLv20.aspx
Download and read the Developer's Guide to get a solid understanding of how the solution was configured
to work with the ComponentSpace component.


To install/configure the demo:
------------------------------
1. Pull the source files from GitHub at https://github.com/OneCare/OneCare.SAML.Demo and put them in any directory.
2. Import the 3 Raven databases in the Databases folder (using Raven Studio).  They are OneCarePartnerIdP, 
OneCarePartnerSP, and OneCarePortalSP
3. Open the OneCare.SAML.Demo.sln solution and build everything.
4. Create 3 web applications using IIS Manager -- OneCare.Partner.IdP, OneCare.Partner.SP, and 
OneCare.Portal.SP


Here are some usage steps/notes:
--------------------------------
Start From Scratch Steps
1. Open a browser and navigate to http://localhost/OneCare.Partner.SP (notice that you are not logged in)
2. Click the login link
3. Notice that you have been taken to the OneCare.Partner.IdP site and it's asking you to login.  Login as 
joe@partnerIdP/joe999
4. Click the Login button and notice that you are taken back to the OneCare.Partner.SP site and that it says you 
are logged in as joe@partnerSP (even though you logged in as joe@partnerIdP -- because these 2 apps are partners 
the user associations have already been made)
5. Also notice that there is now a "Go To OneCare" link in the upper right corner.  Click on this link.
6. You are taken to the OneCare.Portal.SP site (that's us) and it's asking you to associate a OneCare user with 
the Partner SP application
7. Log in as joe/joe999 (a user stored in "our" user database)
8. Notice you are at the OneCare Portal site and it says you are logged in as joe.
9. Now that you've made the association, you shouldn't have to log into the OneCare Portal SP app again.


Clear everything as follows:
----------------------------
Clearing Steps
1. On the OneCare.Portal.SP site, click Logout
2. Go to http://localhost/OneCare.Partner.SP and click Logout
3. Go to http://localhost/OneCare.Partner.IdP and click Logout
4. Close the browser.


To check that the association sticks:
-------------------------------------
Verify Association Steps
1. Open the browser and go to http://localhost/OneCare.Partner.SP, click Login and login to the IdP site 
as joe@partnerIdP/joe999 (you can also confirm that you wouldn't have to do this if you hadn't taken step 
3 above -- in that case you'd already be logged into the IdP and when you clicked the Login link in the Partner 
SP site, you would simply have been logged in as joe@partnerSP with no need to enter credentials)
4. From the OneCare.Partner.SP site, notice that you are logged in as joe@partnerSP, click the "Go To OneCare" link
5. Notice that you are now at the OneCare.Portal.SP site and are logged in as "joe".


If you want to clear everything out and start from scratch,
-----------------------------------------------------------
you will need to modify the UserDocs/1 document in the OneCarePortalSP database using Raven Studio.  In that 
document, simply set the ExternalAccounts field to "null" and Save.  Now follow the Clearing Steps listed above 
then start from the top.


Here are some things that still need to be considered/fixed/improved:
---------------------------------------------------------------------
1. Everything should be using SSL to protect the information that is flowing through the URLs
2. There is no provision for failing to login or for opting out of logging in.  We need to figure out how to send 
SAML back to the caller that says the user couldn't or wouldn't be authenticated.
3. We'd probably want to prevent DOS attacks at the PartnerSSO/Login endpoint by preventing multiple identical 
calls from coming in


Assuming this demo is close to home as far as our partner's IdPs are concerned, here's what it would take to 
configure the real OneCare Portal with a Partner Application
-------------------------------------------------------------------------------------------------------------
1. Configure the Partner IdP to understand that OneCare can make SAML requests.  This would likely, at a minimum, 
require that we produce an X509 certificate and hand them the public key (or .cer file).  It will also likely 
entail configuring the SAML endpoint that their IdP calls on our side
2. Configure the OneCare application:
  a. add an endpoint that the partner application can call passing the app name and password using basic 
  authentication (BA)
  b. read the partner app credentials from the BA
  c. we should previously have made an ExternalAppDoc record for the app that we can match to the partner app's 
  credentials and that record should contain an entry for the IdP we should call (in the case of the demo, it 
  simply contains the name of the IdP that is configured in the saml.config file, which is all that ComponentSpace 
  needs to make the actual SAML call)
  d. Generate an official X509 certificate and send the .cer file to the IdP company
3. Configure the Partner application:
  a. Add a OneCare link that calls a OneCareController method that has been added to their application
  b. Help the partner write the OneCareController method so that it invokes the OneCare PartnerSSO endpoint 
  discussed above and passes the partner credentials using BA


contact sam.freeman.55@gmail.com for more information.
