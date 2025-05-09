using System.Text.RegularExpressions;
using TavernSystem.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using TavernSystem.Application;

namespace TavernSystem.Repositories;

public class AdventurerRepository : IAdventurerRepository
{
    private readonly string _connectionString;

    public AdventurerRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IEnumerable<object> GetAllAdventurers()
    {
        List<object> adventurers = [];

        const string sql = "SELECT Id, Nickname FROM Adventurer";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(sql, connection);

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        adventurers.Add(new
                        {
                            Id = reader.GetInt32(0),
                            Nickname = reader.GetString(1),
                        });
                    }
                }
            }
            finally
            {
                reader.Close();
            }
        }

        return adventurers;
    }

    public ReturenAdventurerDTO? GetAdventurerById(int id)
    {
        ReturenAdventurerDTO? adventurer = null;

        string sql = @"
                SELECT 
                    a.Id, a.Nickname, r.Name AS RaceName, e.Name AS ExperienceLevel,
                    p.Id AS PersonId, p.FirstName, p.MiddleName, p.LastName, p.HasBounty
                FROM Adventurer a
                JOIN Race r ON a.RaceId = r.Id
                JOIN ExperienceLevel e ON a.ExperienceId = e.Id
                JOIN Person p ON a.PersonId = p.Id
                WHERE a.Id = @id";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        adventurer = new ReturenAdventurerDTO()
                        { Id = reader.GetInt32(0), Nickname = reader.GetString(1), Race = reader.GetString(2), Experience = reader.GetString(3),
                            PersonData = new Person()
                            { Id = reader.GetString(4), FirstName = reader.GetString(5), MiddleName = reader.IsDBNull(6) ? null : reader.GetString(6), LastName = reader.GetString(7),
                                HasBounty = reader.GetBoolean(8)
                            }
                        };
                    }
                }
            }
        }

        return adventurer;
    }
    
    public bool CreateAdventurer(AdventurerDTO adventurer)
        {
            if (adventurer == null)
                return false;

            if (!IsValidPersonId(adventurer.PersonId))
                return false;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        if (HasAnyBounties(adventurer.Nickname))
                        {
                            transaction.Rollback();
                            return false; 
                        }

                        string sql = @"
                            INSERT INTO Adventurer (Nickname, RaceId, ExperienceId, PersonId)
                            VALUES (@nickname,  @raceId, @experienceId, @personId)";

                        using (SqlCommand com = new SqlCommand(sql, connection, transaction))
                        {
                            com.Parameters.AddWithValue("@nickname", adventurer.Nickname);
                            com.Parameters.AddWithValue("@raceId", adventurer.RaceId);
                            com.Parameters.AddWithValue("@experienceId", adventurer.ExperienceId);
                            com.Parameters.AddWithValue("@personId", adventurer.PersonId);

                            int rowsAffected = com.ExecuteNonQuery();
                            if (rowsAffected == 1)
                            {
                                transaction.Commit();
                                return true;
                            }
                        }

                        transaction.Rollback();
                        return false;
                    }
                    catch
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        public bool HasAnyBounties(string adventurerName)
        {
            int? bounty = null;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT HasBounty FROM Person JOIN Adventurer ON Person.Id = Adventurer.PersonId WHERE Adventurer.Name = @adventurerName";
                connection.Open();
                using (SqlCommand com = new SqlCommand(sql, connection))
                {
                    com.Parameters.AddWithValue("@adventurerName", adventurerName);
                    bounty = (int?)com.ExecuteScalar();
                }
            }
            return bounty != 0;
        }

        private bool IsValidPersonId(string personId)
        {
            string pattern = @"^[A-Z]{2}\d{4}(0[1-9]|1[0-1])(0[1-9]|1\d|2[0-8])\d{4}[A-Z]{2}$";
            if (!Regex.IsMatch(personId, pattern))
                return false;

            return true;
        }
}
