<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/ProjectView.Master" AutoEventWireup="true"
    CodeBehind="ProjectView.aspx.cs" Inherits="ProjectPilot.Portal.Views.ProjectView" %>
<%@ Import Namespace="ProjectPilot.Framework.Modules"%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ModuleListPlaceHolder" runat="server">
    <table class="maincontent" cellspacing="0" cellpadding="0" border="0">
        <tbody>
            <tr>
                <td class="sidebar">
                    <asp:ListView ID="ProjectModulesList" runat="server">       
                        <LayoutTemplate>
                            <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
                        </LayoutTemplate>
                        <%-- comment <fds> fsd --%>
                        
                        <ItemTemplate>
                            <div class="sectionlink">
                                <p class="inactive_indent">
<%# Html.ActionLink((string)Eval("ModuleName"), String.Format("Module/{0}/{1}", this.ViewData.Model.Project.ProjectId, (string)Eval("ModuleId")), "ProjectView")%>   
                            </div>
                        </ItemTemplate>
                    </asp:ListView>
                    <%-- multi line
                    comment 
                    one more line --%>
                </td>
            </tr>
        </tbody>
    </table>
</asp:Content>

<%-- comment --%>
<%-- multi line
comment 
one more line --%>
<asp:Content ID="Content3" ContentPlaceHolderID="ModulePlaceHolder" runat="server">
    <% 
        if (ViewData.Model.ModuleId != null)
        {
            IProjectModule moduleToShow = ViewData.Model.Module;
            IViewable viewableModule = (IViewable)moduleToShow;
            Response.Write(viewableModule.FetchHtmlReport());
        }
%>
</asp:Content>