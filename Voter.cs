using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
namespace VotingMachine
{
   public class Voter
    {
        Candidate can;// use to Get The partname from Candidate class
        private string VoterName;
        private string cnic;
        private string selectedpartyname;
        public Voter(string voterName, string cnic, string selectedpartyname)
        {
            this.VoterName = voterName;
            this.cnic = cnic;
            this.selectedpartyname = selectedpartyname;
        }
        // I use this constructor so make an object for this class
        public Voter() { }
        // I can make setters to set the value for using the cast vote method

    public void setVoterName(string name)
        {
            this.VoterName = name;
        }
        public string getVoterName() {  return this.VoterName; }

        public void setCNIC(string CNIC) { 
            this.cnic = CNIC;
        }
      public string getCnic()
        {
            return cnic;
        }
        public string SelectedPartyName() {

            
            return can.CandidatePartyName;
        }
        public bool hasVoted(string cnic)
        {
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Candidate;Integrated Security=True;Connect Timeout=30;Encrypt=False;";
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string query = $"SELECT * FROM Voter WHERE cnic = '{cnic}'";
            SqlCommand cmd = new SqlCommand(query,connection);
            SqlDataReader dr = cmd.ExecuteReader();
            while(dr.Read())
            {
                if (dr["cnic"].ToString()==cnic)
                {
                    return true;
                }
            }
            connection.Close();
            return false;
        }
    }
}
