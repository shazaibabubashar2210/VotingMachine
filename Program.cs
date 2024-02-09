using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingMachine
{
     public class Program
    {
        static void Main(string[] args)
        {
            VotingMachine machine = new VotingMachine();
            Candidate c = new Candidate();
            Voter v = new Voter();
               
            Console.WriteLine("----------------------------------------Welcome To Online Voting Machine-------------------------------------------");
            while (true)
            {
                Console.WriteLine("1. Add Voter");
                Console.WriteLine("2. Update Voter");
                Console.WriteLine("3. Delete Voter");
                Console.WriteLine("4. Display Voters");
                Console.WriteLine("5. Cast Vote");
                Console.WriteLine("6. Insert Candidate");
                Console.WriteLine("7. Update Candidate");
                Console.WriteLine("8. Display Candidates");
                Console.WriteLine("9. Delete Candidate");
                Console.WriteLine("10. Declare Winner");
                Console.WriteLine("-1. Exit");
                Console.Write("Enter your choice from 1 to 10: ");
                Int64 choice=Convert.ToInt64(Console.ReadLine());
                if (choice==1)
                {
                    machine.addVoter();
                }
                else if (choice==2)
                {
                    Console.Write("Enter CNIC Of The Voter to Update: ");
                    
                    string cnic = Console.ReadLine();
                    machine.updateVoter(cnic);
                }
                else if (choice==3)
                {
                    Console.Write("Enter CNIC To Delete Voter: ");
                    string cnic= Console.ReadLine();
                    machine.deleteVoter(cnic);
                }
                else if (choice == 4)
                {
                    machine.displayVoters();
                }
                else if (choice == 5)
                {
                    machine.CastVote(c, v);
                }
                else if (choice==6)
                {
                    machine.insertCandidate(c);
                }
                else if(choice==7)
                {
                    Console.Write("Enter id to Update Candidate: ");
                    int id=Convert.ToInt32(Console.ReadLine());
                    machine.updateCandidateFromDataBase(c, id);
                }
                else if (choice == 8)
                {
                    machine.displayCaniddates();
                }
                else if (choice==9)
                {
                    Console.Write("Enter id to Delete Candidate: ");
                    int id=Convert.ToInt32(Console.ReadLine());
                    machine.deleteCandidate(id);

                }
                else if (choice == 10)
                {
                    machine.DeclareWinner();
                }
                else if (choice > 11)
                {
                    Console.WriteLine("----------------------------------------");
                    Console.WriteLine("Wrong! Plz Select the availabe choice");
                    Console.WriteLine("----------------------------------------");
                }
                else if (choice == -1)
                {
                    Console.WriteLine("----------------------------------------");
                    Console.WriteLine("ThankYou! For Using Our Voting Machine");
                    Console.WriteLine("----------------------------------------");
                    break;
                }
            }
        }
    }
}
