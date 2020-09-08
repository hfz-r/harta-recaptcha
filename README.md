# Harta-Recaptcha

[![harta-recaptcha package in harta-recaptcha-feed@Release feed in Azure Artifacts](http://bj-tfs:8080/tfs/DefaultCollection/_apis/public/Packaging/Feeds/43088a81-ed89-4f08-bdd2-510978b78c59@8920759d-f5d5-4f76-9d18-e45ecb83ac24/Packages/4cd6cf18-61d2-4aed-bc87-2445872b9017/Badge)](http://bj-tfs:8080/tfs/DefaultCollection/Harta-Recaptcha/_packaging?_a=package&feed=43088a81-ed89-4f08-bdd2-510978b78c59%408920759d-f5d5-4f76-9d18-e45ecb83ac24&package=4cd6cf18-61d2-4aed-bc87-2445872b9017&preferRelease=true)
[![Build Status](http://bj-tfs:8080/tfs/DefaultCollection/Harta-Recaptcha/_apis/build/status/harta-recaptcha-ci?branchName=master)](http://bj-tfs:8080/tfs/DefaultCollection/Harta-Recaptcha/_build/latest?definitionId=4&branchName=master)

Google reCAPTCHA V2 wrapper for the ASP.NET Web Forms (.NET Framework>4.0) with ***online/offline detection*** capabilities.

# Install

From a **Package Manager Console** or **PowerShell**; install .nupkg locally
```powershell
# Installs harta-recaptcha 1.0.3 package, using the .nupkg file under local path of c:\temp\packages
Install-Package c:\temp\packages\harta-recaptcha.1.0.3.nupkg
```

```powershell
# Installs harta-recaptcha but not its dependencies from c:\temp\packages
Install-Package harta-recaptcha -IgnoreDependencies -Source c:\temp\packages
```

# Requirements
You must first have a **secret key** and a **site key** in order to use the reCAPTCHA service. This package supports v2 api keys. You can read more about reCAPTCHA v2, and sign up for free here: https://www.google.com/recaptcha/intro/

# Configure

Configuration can be setup from the application or page level depends on the situation.

#### web.config
Add the following entry to the file and make sure to paste in your secret key and site key:
```xml
<add key="RecaptchaSecretKey" value="paste secret key here" />
<add key="RecaptchaSiteKey" value="paste site key here" />
```

Optionally, if you want to disable the control, add following entry to the file. (Default value = false) 
```xml
<add key="RecaptchaSkipValidation" value="true|false:boolean" /> 
```

#### *.aspx
Inject package assembly and namespace within the directive syntax as described below:  
```html
<%@ Register Assembly="Recaptcha" Namespace="Recaptcha" TagPrefix="recaptcha" %>
```

Place the control within form tags. Optionally, **SkipRecaptcha** can be use on this level to skip the validation procedure *aka* disable the control. 
```html
<recaptcha:RecaptchaControl SiteKey="Your site key" SecretKey="Your secret key" SkipRecaptcha="true|false" runat="server"/>
```

# Usage

TLDR: [Complete example](https://github.com/hfz-r/harta-recaptcha/tree/master/test) 

*Note: Example below will assumed that the [configuration](#Configure) was done on the application level*

Inject control on selected page
```html
<%@ Register Assembly="Recaptcha" Namespace="Recaptcha" TagPrefix="recaptcha" %>
```

Place custom tag derived from the registered control within *form*.
```html
<form id="form1" runat="server">
    <div>
        <recaptcha:RecaptchaControl runat="server"/>
    </div>
</form>
```

Create ```HiddenField``` on the same page and hook the ID to the recaptcha control attribute ```HiddenFieldControlId```
*Note: HiddenField used to store the browser connectivity state*
```html
<form id="form1" runat="server">
    <asp:HiddenField runat="server" ID="HiddenFieldID"/>
    <div>
        <recaptcha:RecaptchaControl HiddenFieldControlId="HiddenFieldID" runat="server"/>
    </div>
</form>
```

Final form should be like below:
```html
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="RecaptchaTest._Default" %>
<%@ Register Assembly="Recaptcha" Namespace="Recaptcha" TagPrefix="recaptcha" %>
<!DOCTYPE html>
<html>
<body>
 <form id="form1" runat="server">
    <asp:HiddenField runat="server" ID="BrowserState"/>
    <div>
        <recaptcha:RecaptchaControl ID="RecaptchaControl" HiddenFieldControlId="BrowserState" runat="server"/>
        <asp:Label ID="RecaptchaResult" runat="server"/><br/>
        <asp:Button ID="RecaptchaButton" Text="Submit" runat="server" onclick="RecaptchaButton_Click"/>
    </div>
 </form>
</body>
</html>
```

# Validation

To validate the recaptcha attempts, use ```Page.IsValid``` on the backend logic. This property will trigger the validation pipeline and send the secured request with provided keys to the upstream server for validation (spam/etc.) 
```cs
protected void RecaptchaButton_Click(object sender, EventArgs e)
{
	if (this.Page.IsValid)
	{
		this.RecaptchaResult.Text = "Success";
	}
	else
	{
		this.RecaptchaResult.Text = this.RecaptchaControl.ErrorMessage;
	}
}
```

Invalid response will be attached to ```ErrorMessage``` property, by leveraging default API response from [Google reCAPTCHA](https://developers.google.com/recaptcha/docs/verify#error_code_reference)
