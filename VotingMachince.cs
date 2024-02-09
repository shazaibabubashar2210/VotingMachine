using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.IO.Pipes;
using System.IO.Ports;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

namespace VotingMachine
{
    public class VotingMachine
    {
        private List<Candidate> candidates;

        public VotingMachine()
        {
            candidates = new List<Candidate>();
        }

        public void CastVote(Candidate c, Voter v)
        {
            Console.Write("Enter Your CNIC: ");
            string cnic = Console.ReadLine();
            Console.Write("Enter your name: ");
            string name=Console.ReadLine();
            v.setVoterName(name);
            if (!v.hasVoted(cnic))
            {
                string connectionStringForInsertingData = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Candidate;Integrated Security=True;Connect Timeout=30;Encrypt=False;";
                // Check if the voter with the same CNIC already exists
                string checkVoterQuery = $"SELECT COUNT(*) FROM Voter WHERE cnic = '{cnic}'";
                using (SqlConnection checkVoterConnection = new SqlConnection(connectionStringForInsertingData))
                {
                    checkVoterConnection.Open();
                    SqlCommand checkVoterCmd = new SqlCommand(checkVoterQuery, checkVoterConnection);
                    int existingVoterCount = (int)checkVoterCmd.ExecuteScalar();
                    checkVoterConnection.Close();

                    if (existingVoterCount > 0)
                    {
                        Console.WriteLine("--------------------------------------------------------");
                        Console.WriteLine("Error: Voter with the same CNIC already exists.");
                        Console.WriteLine("--------------------------------------------------------");
                        return; // Exit the method if the voter already exists
                    }
                }

                Console.Write("Enter Candidate Name: ");
                string cname = Console.ReadLine();

                Console.Write("Enter Candidate Party Name you want to vote for? ");
                string CnPartyName = Console.ReadLine();


                c.CandidateName = cname;
                c.CandidatePartyName = CnPartyName;
                int id = c.GetCandidateId();
                int votes = c.IncrementVotes();

                InsertIntoFile(c);
                using (SqlConnection connection = new SqlConnection(connectionStringForInsertingData))
                {
                    connection.Open();

                    // Check if the candidate exists
                    string query1 = $"SELECT * FROM candidate WHERE Name = '{c.CandidateName}' AND Party = '{c.CandidatePartyName}'";
                    SqlCommand cmd1 = new SqlCommand(query1, connection);
                    SqlDataReader dr1 = cmd1.ExecuteReader();

                    if (dr1.Read())
                    {
                        dr1.Close();

                        // Increment votes in the database
                        
                        string updateQuery = $"UPDATE candidate SET Votes = Votes+1 WHERE Name = '{c.CandidateName}' AND Party = '{c.CandidatePartyName}'";

                        SqlCommand updateCmd = new SqlCommand(updateQuery, connection);
                        int rowsAffected = updateCmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // Insert voter record
                            string query3 = $"INSERT INTO Voter(cnic, voterName, SelectedPartyName) VALUES ('{cnic}', '{v.getVoterName()}', '{CnPartyName}')";
                            SqlCommand cmd3 = new SqlCommand(query3, connection);
                            int rowEffected3 = cmd3.ExecuteNonQuery();

                            if (rowEffected3 > 0)
                            {
                                Console.WriteLine("--------------------------------------------------------");
                                Console.WriteLine("Successfully cast a vote!!!");
                                Console.WriteLine("--------------------------------------------------------");
                            }
                            else
                            {
                                Console.WriteLine("--------------------------------------------------------");
                                Console.WriteLine("Error inserting voter record");
                                Console.WriteLine("--------------------------------------------------------");
                            }
                        }
                        else
                        {
                            Console.WriteLine("--------------------------------------------------------");
                            Console.WriteLine("Error updating candidate votes");
                            Console.WriteLine("--------------------------------------------------------");
                        }
                    }
                    else
                    {
                        Console.WriteLine("--------------------------------------------------------");
                        Console.WriteLine("Candidate does not exist. Vote not cast.");
                        Console.WriteLine("--------------------------------------------------------");
                    }

                    // Close the connection explicitly
                    connection.Close();
                }

                candidates.Add(c);
            }
            else
            {
                Console.WriteLine("--------------------------------------------------------");
                Console.WriteLine("You have already voted. Multiple votes are not allowed.");
                Console.WriteLine("--------------------------------------------------------");
            }
        }

        public void addVoter()
        {
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("1: Add Voter");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("Enter Voter Details:");
            Console.Write("Name: ");
            string voterName = Console.ReadLine();
            Console.Write("CNIC: ");
            string voterCnic = Console.ReadLine();
            Console.Write("PartyName: ");
            string voterPartyName = Console.ReadLine();

            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Candidate;Integrated Security=True;Connect Timeout=30;Encrypt=False;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Check if the voter with the same CNIC already exists
                string checkVoterQuery = $"SELECT COUNT(*) FROM Voter WHERE cnic = '{voterCnic}'";

                using (SqlCommand checkVoterCmd = new SqlCommand(checkVoterQuery, connection))
                {
                    int existingVoterCount = (int)checkVoterCmd.ExecuteScalar();

                    if (existingVoterCount > 0)
                    {
                        Console.WriteLine("-------------------------------");
                        Console.WriteLine("Error: Voter with the same CNIC already exists.");
                        Console.WriteLine("-------------------------------");
                        return; // Exit the method if the voter already exists
                    }
                }
                string insertQuery = $"INSERT INTO voter(cnic, VoterName, SelectedPartyName) VALUES ('{voterCnic}', '{voterName}', '{voterPartyName}')";
                using (SqlCommand insertCmd = new SqlCommand(insertQuery, connection))
                {
                    int rowsAffected = insertCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("-------------------------------");
                        Console.WriteLine("Voter Added Successfully!");
                        Console.WriteLine("-------------------------------");
                    }
                    else
                    {
                        Console.WriteLine("-------------------------------");
                        Console.WriteLine("Error");
                        Console.WriteLine("-------------------------------");
                    }
                }
            }

            StreamWriter streamWriter = new StreamWriter("Voter.txt", true);
            streamWriter.WriteLine($"{voterCnic},{voterName},{voterPartyName}");
            streamWriter.Close();
            Console.WriteLine("------------------------------------");
            Console.WriteLine("Successfully Write in a Text File");
            Console.WriteLine("------------------------------------");
        }

        public void updateVoter(string cnic)
        {
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("2: Update Voter");
            Console.WriteLine("--------------------------------------------------");
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Candidate;Integrated Security=True;Connect Timeout=30;Encrypt=False;";
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            Console.WriteLine("Enter new voter details:");
            Console.Write("Enter Voter Name: ");
            string voterName = Console.ReadLine();
            string query = $"Update Voter set VoterName='{voterName}' where CNIC='{cnic}'";
            
            SqlCommand cmd = new SqlCommand(query, conn);
            int rowEffected = cmd.ExecuteNonQuery();
            if (rowEffected > 0)
            {
                Console.WriteLine("------------------------------------------");
                Console.WriteLine("Voter Update Successfully!");
                Console.WriteLine("------------------------------------------");
            }
            else
            {
                Console.WriteLine("------------------------------------------");
                Console.WriteLine("Error Updating Voter");
                Console.WriteLine("------------------------------------------");
            }
            conn.Close();

            // Updating Voter In File
            using (StreamWriter sw = new StreamWriter("NewVoter.txt", true))
            {
                using (StreamReader sr = new StreamReader("Voter.txt"))
                {
                    bool flag = false;
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        string[] data = line.Split(',');

                        // Check if the array has enough elements before accessing them
                        if (data.Length > 2 && data[0].ToString() == cnic)
                        {
                            sw.WriteLine($"{data[0]},{voterName},{data[2]}");
                            flag = true;
                        }
                        else
                        {
                            sw.WriteLine(line);
                        }
                    }

                    if (flag)
                    {
                        Console.WriteLine("----------------------------------");
                        Console.WriteLine("File Updated successfully!");
                        Console.WriteLine("----------------------------------");
                    }
                } 
            } 
            File.Replace("NewVoter.txt", "Voter.txt", null);
        }
        public void displayVoters()
        {
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("4: Display Voters");
            Console.WriteLine("--------------------------------------------------");
            string connectionString= @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Candidate;Integrated Security=True;Connect Timeout=30;Encrypt=False;";
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            string query = "Select * from voter";
            SqlCommand cmd = new SqlCommand(query,conn);
            SqlDataReader dr = cmd.ExecuteReader();
            Console.WriteLine("------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("List Of Voters: ");
            while(dr.Read())
            {
                Console.WriteLine($"VoterName->{dr[1]} CNIC-> {dr[0]} SELECTED Party Name-> {dr[2]}");
            }
            Console.WriteLine("------------------------------------------------------------------------------------------------------------------");
            conn.Close();
        }


        public void displayCaniddates()
        {
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("8: Display Candidate");
            Console.WriteLine("--------------------------------------------------");
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Candidate;Integrated Security=True;Connect Timeout=30;Encrypt=False;";
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            string query = "Select * from candidate";
            SqlCommand cmd = new SqlCommand(query, conn);
            SqlDataReader dr = cmd.ExecuteReader();
            Console.WriteLine("------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("List Of Candidates: ");
            while (dr.Read())
            {
                Console.WriteLine($"CandidateId->{dr[0]} Name-> {dr[1]} Party Name-> {dr[2]} Votes-> {dr[3]}");
            }
            Console.WriteLine("------------------------------------------------------------------------------------------------------------------");
            conn.Close();
        }
          


        public void deleteVoter(string cnic)
        {
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("3: Delete Voter");
            Console.WriteLine("--------------------------------------------------");
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Candidate;Integrated Security=True;Connect Timeout=30;Encrypt=False;";
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            string query = $"Delete from voter where cnic='{cnic}'";
            SqlCommand cmd= new SqlCommand(query, conn);
            int rowEffected = cmd.ExecuteNonQuery();
            if (rowEffected > 0)
            {
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine("Voter Deleted Successfully!");
                Console.WriteLine("---------------------------------------------");
            }
            else
            {
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine("Error! Deleting Voter...");
                Console.WriteLine("---------------------------------------------");
                return;
            }
            conn.Close();

            using (StreamWriter sw = new StreamWriter("temp.txt", true))
            {
                using (StreamReader sr = new StreamReader("Voter.txt"))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        string[] data = line.Split(',');

                        if (data.Length > 2 && data[0] != cnic)
                        {
                            sw.WriteLine($"'{data[0]}','{data[1]}','{data[2]}'");
                        }
                    }
                }
            }

            File.Delete("Voter.txt");
            File.Move("temp.txt", "Voter.txt");

            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Successfully Deleted a Voter!");
            Console.WriteLine("----------------------------------------");
        }

        public void insertCandidate(Candidate c)
        {
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("6: Insert Candidate");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("Enter Candidate Details: ");
            Console.Write("Name: ");
            string name = Console.ReadLine();
            Console.Write("Party: ");
            string party = Console.ReadLine();
         
            c.CandidateName = name;
            c.CandidatePartyName = party;
            int id = c.GetCandidateId();
            int votes = c.IncrementVotes();
            InsertIntoFile(c);

            string connectionString= @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Candidate;Integrated Security=True;Connect Timeout=30;Encrypt=False;";
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
       

           string query = $"INSERT INTO Candidate (CandidateId, Name, Party,Votes) VALUES ('{c.GetCandidateId()}', '{name}', '{party}',{0})";

            SqlCommand cmd = new SqlCommand(query,conn);
            int rowEffected = cmd.ExecuteNonQuery();
            if (rowEffected>0)
            {
                Console.WriteLine("-------------------------------------");
                Console.WriteLine("Candidate Inserted Successfully!");
                Console.WriteLine("-------------------------------------");
            }
            else
            {
                Console.WriteLine("--------------------------------------");
                Console.WriteLine("Error! Inserted Candidate");
                Console.WriteLine("--------------------------------------");
            }
            conn.Close();

          
        }

        // Inserting a Candidate in a File
        public void InsertIntoFile(Candidate c)
        {
            var candidateData = new { CandidateId = c.GetCandidateId(), CandidateName = c.CandidateName, CandidatePartyName = c.CandidatePartyName,votes=c.IncrementVotes() };

            string outputString = JsonSerializer.Serialize(candidateData);

            using (StreamWriter sw = new StreamWriter("Candidate.txt", true))
            {
                sw.WriteLine(outputString);
                Console.WriteLine("------------------------------------------------------");
                Console.WriteLine("Successfully Inserted a Candidate in a File...");
                Console.WriteLine("------------------------------------------------------");
            }
        }


        // Declare Winner Method
        public void DeclareWinner()
        {
            int maxVotes = 0;
            string winnerName = "";
            string winnerParty = "";

            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Candidate;Integrated Security=True;Connect Timeout=30;Encrypt=False;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT CandidateID, Name, Party, Votes FROM Candidate";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        int candidateId = Convert.ToInt32(dr["CandidateID"]);
                        string candidateName = dr["Name"].ToString();
                        string candidateParty = dr["Party"].ToString();
                        int votes = Convert.ToInt32(dr["Votes"]);

                        if (votes > maxVotes)
                        {
                            maxVotes = votes;
                            winnerName = candidateName;
                            winnerParty = candidateParty;
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(winnerName))
            {
                Console.WriteLine("------------------------------------------------------");
                Console.WriteLine("Winner Details:");
                Console.WriteLine($"Candidate Name: {winnerName}");
                Console.WriteLine($"Candidate Party: {winnerParty}");
                Console.WriteLine($"Votes: {maxVotes}");
                Console.WriteLine("------------------------------------------------------");
            }
            else
            {
                Console.WriteLine("------------------------------------------------------");
                Console.WriteLine("No winner declared. No candidates found.");
                Console.WriteLine("------------------------------------------------------");
            }
        }
        public void readCandidate(int id)
        {
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine("Reading From Data Base Thorugh Id");
            Console.WriteLine("-------------------------------------------------");
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Candidate;Integrated Security=True;Connect Timeout=30;Encrypt=False;";
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            string query = $"Select * from Candidate where CandidateID={id}";
            SqlCommand cmd = new SqlCommand(query,conn);
            SqlDataReader dr = cmd.ExecuteReader();

            Console.WriteLine("-----------------------------------------------------------------------------------------------");
            if(dr.HasRows)
            {
               while(dr.Read())
                 {
                     Console.WriteLine($"Candidate Id: {dr[0]} Name: {dr[1]} Party: {dr[2]} Votes: {dr[3]}");
                 }
            }
            else
            {
                Console.WriteLine($"Candidate with {id} does'nt exist!");
            }
            Console.WriteLine("-----------------------------------------------------------------------------------------------");
            conn.Close();
            ReadCandidateFromFile(id);
        }
        public void ReadCandidateFromFile(int id)
        {
            string filePath = "Candidate.txt";
            bool flag = false;
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"{filePath} does not exist.");
                return;
            }

            using (StreamReader dr = new StreamReader(filePath))
            {
                while (!dr.EndOfStream)
                {
                    string line = dr.ReadLine();
                    Candidate c = JsonSerializer.Deserialize<Candidate>(line);

                    if (c != null && c.GetCandidateId() == id)
                    {
                        Console.WriteLine($"Candidate Id: {c.GetCandidateId()}, Name: {c.CandidateName}, Party Name: {c.CandidatePartyName}");
                        flag = true;
                        break;
                    }
                }
            }
            if (!flag)
            {
                Console.WriteLine("Candidate Not exist!");
            }
        }
        public void updateCandidateFromDataBase(Candidate c, int id)
        {
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Candidate;Integrated Security=True;Connect Timeout=30;Encrypt=False;";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            Console.Write("Enter new Candidate name: ");
            string newName = Console.ReadLine();
            Console.Write("Enter new Party Name: ");
            string newParty = Console.ReadLine();
            c.CandidateName= newName;
            c.CandidatePartyName = newParty;
            string query = $"Update Candidate SET Name='{newName}' , Party='{newParty}' where CandidateID='{id}'";
            SqlCommand cmd = new SqlCommand(query,sqlConnection);
            int rowEffected = cmd.ExecuteNonQuery();
            if (rowEffected>0)
            {
                Console.WriteLine("----------------------------------------------");
                Console.WriteLine("Candidate Updated Successfully!");
                Console.WriteLine("----------------------------------------------");
            }
            else
            {
                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine("Candidate don't update with this provided id");
                Console.WriteLine("---------------------------------------------------");
            }
            sqlConnection.Close();
            updateCandidateFromFile(id,newName,newParty);
        }
        // Update Candidate from file
        public void updateCandidateFromFile(int id, string newName, string newParty)
        {
            string tempFilePath = "temp.txt";
            string filePath = "Candidate.txt";

            using (StreamWriter sw = new StreamWriter(tempFilePath, true))
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        string[] data = line.Split(',');

                        // Check if the first element (CandidateId) can be successfully parsed to an integer
                        if (data.Length > 3 && int.TryParse(data[0], out int candidateId) && candidateId == id)
                        {
                            sw.WriteLine($"{id},{newName},{newParty},{data[3]}");
                        }
                        else
                        {
                            sw.WriteLine(line);
                        }
                    }
                }
            }

            File.Replace(tempFilePath, filePath, null);
        }
        public void deleteCandidate(int id)
        {
          string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Candidate;Integrated Security=True;Connect Timeout=30;Encrypt=False;";
          SqlConnection conn=new SqlConnection(connectionString);
            conn.Open();
            string query = $"Delete from Candidate where CandidateID='{id}'";
            SqlCommand cmd = new SqlCommand(query,conn);
            int rowEffected = cmd.ExecuteNonQuery();
            if(rowEffected > 0)
            {
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine("Successfully Delete Candidate From File...");
                Console.WriteLine("--------------------------------------------------");
            }
            else
            {
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine("Error! Deleting Candidate");
                Console.WriteLine("--------------------------------------------------");
            }
            conn.Close();
            deleteCandidateFromFile(id);
        }

        public void deleteCandidateFromFile(int id)
        {
            string tempFilePath = "temp1.txt";
            string filePath = "Candidate.txt";

            using (StreamReader sr = new StreamReader(filePath))
            {
                using (StreamWriter sw = new StreamWriter(tempFilePath, true))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        string[] data = line.Split(',');

                        if (data.Length > 3 && int.TryParse(data[0], out int candidateId) && candidateId != id)
                        {
                            sw.WriteLine($"{data[0]}, {data[1]}, {data[2]}, {data[3]}");
                        }
                    }
                }
            }

            File.Replace(tempFilePath, filePath, null);
            File.Delete(tempFilePath);
        }

    }
}
