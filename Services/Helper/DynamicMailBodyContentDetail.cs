using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Helper
{
    public class DynamicMailBodyContentDetail
    {
        public static readonly string mailBodyContentDet = @"
         <table style=""border-collapse: collapse; width: 100%; border: 1px solid black;"">
        <tr>
            <td style=""border: 1px solid black; padding: 8px; width: 25%;"">Plant</td>
            <td colspan='3' style=""border: 1px solid black; padding: 8px;"">@Plant</td>
        </tr>
        <tr>
            <td style=""border: 1px solid black; padding: 8px; width: 25%;"">Form Type</td>
            <td style=""border: 1px solid black; padding: 8px; width: 25%;"">@FormType</td>
            @SamplingCheck
        </tr>
        <tr>
            <td style=""border: 1px solid black; padding: 8px; width: 25%;"">Form Number</td>
            <td style=""border: 1px solid black; padding: 8px; width: 25%;"">@FormNumber <a href='@Formlink'> (click for more details)</a></td>
            @Dept
        </tr>
        <tr>
            <td style=""border: 1px solid black; padding: 8px; width: 25%;"">Status</td>
            <td style=""border: 1px solid black; padding: 8px; width: 25%;"">@Status</td>
            @Vendor
        </tr>
        <tr>
            <td style=""border: 1px solid black; padding: 8px; width: 25%;"">Detection Date</td>
            <td style=""border: 1px solid black; padding: 8px; width: 25%;"">@DetectionDate</td>
            @TotalQty
        </tr>
        <tr>
            @Product
            @AffectedCavity
        </tr>
        <tr>
            @MaterialCode
            @NCCategory
        </tr>
        <tr>
            @MaterialDesc
            @NCDescription          
        </tr>
        <tr>
            <td style=""border: 1px solid black; padding: 8px; width: 25%;"">NC Picture</td>
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
