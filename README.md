# Harta-Recaptcha

[![nuget](https://img.shields.io/badge/nuget-v1.0.4-blue)](https://github.com/hfz-r/harta-recaptcha/releases)
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
