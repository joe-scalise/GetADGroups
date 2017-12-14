using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;
using System.IO;

namespace ConsoleApp4
{
    class Programs
    {

        static void Main(string[] args)
        {
            // Test for secrets file or just use
            // string domain = "domain.local";
            string domain = null;
            try
            {
                domain = System.IO.File.ReadAllText(@"C:\temp\secrets\domain.txt");
            }
            catch
            {
                Console.WriteLine("\nThere's a problem with the domain file.");
            }

            if (!(string.IsNullOrEmpty(domain)))
            {

                Console.Write("\nEnter user name: ");
                string userName = Console.ReadLine();
                Console.WriteLine("\nSearching domain " + domain);

                List<string> userGroupList = new List<string>();

                using (var context = new PrincipalContext(ContextType.Domain, domain))
                {
                    using (UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, userName))
                    {
                        if (userPrincipal != null)
                        {
                            PrincipalSearchResult<Principal> list = userPrincipal.GetAuthorizationGroups();

                            int count = 1;

                            var iterGroup = list.GetEnumerator();

                            using (iterGroup)
                            {
                                Console.WriteLine();
                                while (iterGroup.MoveNext())
                                {
                                    try
                                    {
                                        Principal p = iterGroup.Current;
                                        if (!(string.IsNullOrEmpty(p.SamAccountName)))
                                        {
                                            Console.WriteLine(count + ": " + p.SamAccountName.ToLower().Trim());
                                            userGroupList.Add(p.SamAccountName.ToLower().Trim());
                                            count++;
                                        }
                                        else
                                        {
                                            Console.WriteLine("\n* NULL encountered; group " + p.Name + " *\n");
                                        }
                                    }
                                    catch (NoMatchingPrincipalException pex)
                                    {
                                        Console.WriteLine("\n* NoMatchingPrincipalException encountered *\n");
                                        continue;
                                    }
                                }
                            }

                            // Just test that this operation does not fail
                            userGroupList.Sort();

                            Console.WriteLine("\nEnd of list of groups.");
                        }
                        else
                        {
                            Console.WriteLine("\nUser was not found.");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("\nDomain string is missing.");
            }

            Console.WriteLine("\nEnd of program.  Press enter to exit.");
            Console.ReadLine();
        }
    }
}

