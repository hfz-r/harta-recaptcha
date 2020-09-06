<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="RecaptchaTest._Default" %>

<%@ Register Assembly="Recaptcha" Namespace="Recaptcha" TagPrefix="recaptcha" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="icon" type="image/png" href="/favicon-32x32.png" />
    <link rel="icon" type="image/png" href="/favicon-16x16.png" />
    <title></title>
</head>
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