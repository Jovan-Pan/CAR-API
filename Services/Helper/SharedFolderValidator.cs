using Entities.CAR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Services.Helper
{
    public class SharedFolderValidator
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, out IntPtr phToken);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

        public static DirectoryCredentials Validate(string domain, string username, string password, string folderPath)
        {
            DirectoryCredentials result = new DirectoryCredentials();
            result.Message = "You are not authorized to access the shared folder.";

            IntPtr tokenHandle;
            bool loginUser = LogonUser(username, domain, password, 2, 0, out tokenHandle);

            if (!loginUser)
            {
                int errorCode = Marshal.GetLastWin32Error();
                result.Success = false;
                result.Message = $"The user name or password is incorrect for this domain [{domain}].";
            }
            else
            {
                using (WindowsIdentity identity = new WindowsIdentity(tokenHandle))
                {
                    try
                    {
                        bool hasAccess = WindowsIdentity.RunImpersonated(identity.AccessToken, () =>
                        {
                            try
                            {
                                return Directory.Exists(folderPath);
                            }
                            catch (Exception ex)
                            {
                                result.Success = false;
                                result.Message = ex.Message;
                                return false;
                            }
                        });

                        if (hasAccess)
                        {
                            result.Success = true;
                            result.Message = "You are authorized to access the this folder.";
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        CloseHandle(tokenHandle);
                    }
                }
            }

            return result;
        }
    }
}
