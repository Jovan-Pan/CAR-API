using OfficeOpenXml.Style;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Helper
{
    public class MailBodyContentDetail
    {
        public static readonly string mailBodyContentDet = @"
         <table style=""border-collapse: collapse; width: 100%; border: 1px solid black;"">
        <tr>
            <td style=""border: 1px solid black; padding: 8px;"">Plant</td>
            <td colspan='3' style=""border: 1px solid black; padding: 8px;"">@Plant</td>
        </tr>
        <tr>
            <td style=""border: 1px solid black; padding: 8px;"">Form Type</td>
            <td style=""border: 1px solid black; padding: 8px;"">@FormType</td>
            <td style=""border: 1px solid black; padding: 8px;""><b>NC % (Sampling Check)</b></td>
            <td style=""border: 1px solid black; padding: 8px;""><b>@SamplingCheck</b></td>
        </tr>
        <tr>
            <td style=""border: 1px solid black; padding: 8px;"">Form Number</td>
            <td style=""border: 1px solid black; padding: 8px;"">@FormNumber <a href='@Formlink'> (click for more details)</a></td>
            @Dept
        </tr>
        <tr>
            <td style=""border: 1px solid black; padding: 8px;"">Status</td>
            <td style=""border: 1px solid black; padding: 8px;"">@Status</td>
            @Vendor
        </tr>
        <tr>
            <td style=""border: 1px solid black; padding: 8px;"">Detection Date</td>
            <td style=""border: 1px solid black; padding: 8px;"">@DetectionDate</td>
            <td style=""border: 1px solid black; padding: 8px;"">Total Qty</td>
            <td style=""border: 1px solid black; padding: 8px;"">@TotalQty</td>
        </tr>
        <tr>
            <td style=""border: 1px solid black; padding: 8px;"">Product</td>
            <td style=""border: 1px solid black; padding: 8px;"">@Product</td>
            <td style=""border: 1px solid black; padding: 8px;"">Affected Cavity</td>
            <td style=""border: 1px solid black; padding: 8px;"">@AffectedCavity</td>
        </tr>
        <tr>
            <td style=""border: 1px solid black; padding: 8px;"">Material Code</td>
            <td style=""border: 1px solid black; padding: 8px;"">@MaterialCode</td>
            <td style=""border: 1px solid black; padding: 8px;""><b>NC Category</b></td>
            <td style=""border: 1px solid black; padding: 8px;""><b>@NCCategory</b></td>
        </tr>
        <tr>
            <td style=""border: 1px solid black; padding: 8px;""><b>Material Desc</b></td>
            <td style=""border: 1px solid black; padding: 8px;""><b>@MaterialDesc</b></td>
            <td style=""border: 1px solid black; padding: 8px;""><b>NC Description</b></td>
            <td style=""border: 1px solid black; padding: 8px;""><b>@NCDescription</b></td>
        </tr>
        <tr>
            <td style=""border: 1px solid black; padding: 8px;"">NC Picture</td>
            <td colspan='3' style=""border: 1px solid black; padding: 8px;"">
                <div style='display: flex; flex-wrap: wrap;'>
                    @NCPicture
                </div>
            </td>
        </tr>
    </table>
        ";

        //<tr>
        //   <td style = ""border: 1px solid black; padding: 8px;"">Model</td>
        //    <td style = ""border: 1px solid black; padding: 8px;"">@Model</td>
        //    <td style = ""border: 1px solid black; padding: 8px;""></td>
        //    <td style = ""border: 1px solid black; padding: 8px;""></td>
        //</tr>
        //<tr>
        //    <td style = ""border: 1px solid black; padding: 8px;"">Material type</td>
        //    <td style = ""border: 1px solid black; padding: 8px;"">@Materialtype</td>
        //    <td style = ""border: 1px solid black; padding: 8px;""></td>
        //    <td style = ""border: 1px solid black; padding: 8px;""></td>
        //</tr>
    }
}
