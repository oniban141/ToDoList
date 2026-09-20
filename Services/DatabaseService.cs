using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using DailyPlanner.Models;

namespace DailyPlanner.Services
{
    public class DatabaseService : IDisposable
    {
        private readonly SqlConnection _connection;
        
        public DatabaseService(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
            _connection.Open();
        }
        
        public void Dispose()
        {
            if (_connection != null && _connection.State == System.Data.ConnectionState.Open)
            {
                _connection.Close();
            }
            _connection?.Dispose();
        }
        
        #region User Operations
        
        public User GetUserByUsername(string username)
        {
            try
            {
                string query = "SELECT * FROM Users WHERE Username = @Username";
                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = reader.GetInt32("Id"),
                                Username = reader.GetString("Username"),
                                PasswordHash = reader.GetString("PasswordHash"),
                                Email = reader.IsDBNull("Email") ? string.Empty : reader.GetString("Email"),
                                RoleId = reader.GetInt32("RoleId"),
                                GenderId = reader.GetInt32("GenderId"),
                                CreatedAt = reader.GetDateTime("CreatedAt")
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
        
        public User GetUserById(int userId)
        {
            try
            {
                string query = "SELECT * FROM Users WHERE Id = @UserId";
                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = reader.GetInt32("Id"),
                                Username = reader.GetString("Username"),
                                PasswordHash = reader.GetString("PasswordHash"),
                                Email = reader.IsDBNull("Email") ? string.Empty : reader.GetString("Email"),
                                RoleId = reader.GetInt32("RoleId"),
                                GenderId = reader.GetInt32("GenderId"),
                                CreatedAt = reader.GetDateTime("CreatedAt")
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
        
        public int AddUser(User user)
        {
            try
            {
                string query = "INSERT INTO Users (Username, PasswordHash, Email, RoleId, GenderId) VALUES (@Username, @PasswordHash, @Email, @RoleId, @GenderId); SELECT SCOPE_IDENTITY()";
                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    command.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@RoleId", user.RoleId);
                    command.Parameters.AddWithValue("@GenderId", user.GenderId);
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        
        public bool UpdateUser(User user)
        {
            try
            {
                string query = "UPDATE Users SET Username = @Username, PasswordHash = @PasswordHash, Email = @Email, RoleId = @RoleId, GenderId = @GenderId WHERE Id = @Id";
                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Id", user.Id);
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    command.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@RoleId", user.RoleId);
                    command.Parameters.AddWithValue("@GenderId", user.GenderId);
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        
        public bool DeleteUser(int userId)
        {
            try
            {
                string query = "DELETE FROM Users WHERE Id = @Id";
                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Id", userId);
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        
        public List<User> GetAllUsers()
        {
            var users = new List<User>();
            try
            {
                string query = "SELECT * FROM Users";
                using (var command = new SqlCommand(query, _connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User
                            {
                                Id = reader.GetInt32("Id"),
                                Username = reader.GetString("Username"),
                                PasswordHash = reader.GetString("PasswordHash"),
                                Email = reader.IsDBNull("Email") ? string.Empty : reader.GetString("Email"),
                                RoleId = reader.GetInt32("RoleId"),
                                GenderId = reader.GetInt32("GenderId"),
                                CreatedAt = reader.GetDateTime("CreatedAt")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return users;
        }
        
        #endregion
        
        #region Task Operations
        
        public List<Task> GetAllTasks(int userId)
        {
            var tasks = new List<Task>();
            try
            {
                string query = "SELECT * FROM Tasks WHERE UserId = @UserId";
                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tasks.Add(new Task
                            {
                                Id = reader.GetInt32("Id"),
                                Title = reader.IsDBNull("Title") ? string.Empty : reader.GetString("Title"),
                                Description = reader.IsDBNull("Description") ? string.Empty : reader.GetString("Description"),
                                DueDate = reader.IsDBNull("DueDate") ? (DateTime?)null : reader.GetDateTime("DueDate"),
                                Priority = reader.IsDBNull("Priority") ? string.Empty : reader.GetString("Priority"),
                                Status = reader.IsDBNull("Status") ? string.Empty : reader.GetString("Status"),
                                UserId = reader.GetInt32("UserId"),
                                CreatedAt = reader.GetDateTime("CreatedAt")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return tasks;
        }
        
        public Task GetTaskById(int taskId)
        {
            try
            {
                string query = "SELECT * FROM Tasks WHERE Id = @Id";
                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Id", taskId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Task
                            {
                                Id = reader.GetInt32("Id"),
                                Title = reader.IsDBNull("Title") ? string.Empty : reader.GetString("Title"),
                                Description = reader.IsDBNull("Description") ? string.Empty : reader.GetString("Description"),
                                DueDate = reader.IsDBNull("DueDate") ? (DateTime?)null : reader.GetDateTime("DueDate"),
                                Priority = reader.IsDBNull("Priority") ? string.Empty : reader.GetString("Priority"),
                                Status = reader.IsDBNull("Status") ? string.Empty : reader.GetString("Status"),
                                UserId = reader.GetInt32("UserId"),
                                CreatedAt = reader.GetDateTime("CreatedAt")
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
        
        public int AddTask(Task task)
        {
            try
            {
                string query = "INSERT INTO Tasks (Title, Description, DueDate, Priority, Status, UserId) VALUES (@Title, @Description, @DueDate, @Priority, @Status, @UserId); SELECT SCOPE_IDENTITY()";
                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Title", task.Title ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Description", task.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@DueDate", task.DueDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Priority", task.Priority ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Status", task.Status ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UserId", task.UserId);
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        
        public bool UpdateTask(Task task)
        {
            try
            {
                string query = "UPDATE Tasks SET Title = @Title, Description = @Description, DueDate = @DueDate, Priority = @Priority, Status = @Status WHERE Id = @Id";
                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Id", task.Id);
                    command.Parameters.AddWithValue("@Title", task.Title ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Description", task.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@DueDate", task.DueDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Priority", task.Priority ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Status", task.Status ?? (object)DBNull.Value);
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        
        public bool DeleteTask(int taskId)
        {
            try
            {
                string query = "DELETE FROM Tasks WHERE Id = @Id";
                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Id", taskId);
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        
        #endregion
        
        #region Event Operations
        
        public List<Event> GetAllEvents(int userId)
        {
            var events = new List<Event>();
            try
            {
                string query = "SELECT * FROM Events WHERE UserId = @UserId";
                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            events.Add(new Event
                            {
                                Id = reader.GetInt32("Id"),
                                Title = reader.IsDBNull("Title") ? string.Empty : reader.GetString("Title"),
                                Description = reader.IsDBNull("Description") ? string.Empty : reader.GetString("Description"),
                                StartDate = reader.IsDBNull("StartDate") ? (DateTime?)null : reader.GetDateTime("StartDate"),
                                EndDate = reader.IsDBNull("EndDate") ? (DateTime?)null : reader.GetDateTime("EndDate"),
                                Location = reader.IsDBNull("Location") ? string.Empty : reader.GetString("Location"),
                                UserId = reader.GetInt32("UserId"),
                                CreatedAt = reader.GetDateTime("CreatedAt")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return events;
        }
        
        public int AddEvent(Event ev)
        {
            try
            {
                string query = "INSERT INTO Events (Title, Description, StartDate, EndDate, Location, UserId) VALUES (@Title, @Description, @StartDate, @EndDate, @Location, @UserId); SELECT SCOPE_IDENTITY()";
                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Title", ev.Title ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Description", ev.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@StartDate", ev.StartDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@EndDate", ev.EndDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Location", ev.Location ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UserId", ev.UserId);
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        
        public bool UpdateEvent(Event ev)
        {
            try
            {
                string query = "UPDATE Events SET Title = @Title, Description = @Description, StartDate = @StartDate, EndDate = @EndDate, Location = @Location WHERE Id = @Id";
                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Id", ev.Id);
                    command.Parameters.AddWithValue("@Title", ev.Title ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Description", ev.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@StartDate", ev.StartDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@EndDate", ev.EndDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Location", ev.Location ?? (object)DBNull.Value);
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        
        public bool DeleteEvent(int eventId)
        {
            try
            {
                string query = "DELETE FROM Events WHERE Id = @Id";
                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Id", eventId);
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        
        #endregion
        
        #region Note Operations
        
        public List<Note> GetAllNotes(int userId)
        {
            var notes = new List<Note>();
            try
            {
                string query = "SELECT * FROM Notes WHERE UserId = @UserId";
                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            notes.Add(new Note
                            {
                                Id = reader.GetInt32("Id"),
                                Title = reader.IsDBNull("Title") ? string.Empty : reader.GetString("Title"),
                                Content = reader.IsDBNull("Content") ? string.Empty : reader.GetString("Content"),
                                UserId = reader.GetInt32("UserId"),
                                CreatedAt = reader.GetDateTime("CreatedAt"),
                                UpdatedAt = reader.IsDBNull("UpdatedAt") ? (DateTime?)null : reader.GetDateTime("UpdatedAt")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return notes;
        }
        
        public int AddNote(Note note)
        {
            try
            {
                string query = "INSERT INTO Notes (Title, Content, UserId) VALUES (@Title, @Content, @UserId); SELECT SCOPE_IDENTITY()";
                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Title", note.Title ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Content", note.Content ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UserId", note.UserId);
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        
        public bool UpdateNote(Note note)
        {
            try
            {
                string query = "UPDATE Notes SET Title = @Title, Content = @Content, UpdatedAt = @UpdatedAt WHERE Id = @Id";
                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Id", note.Id);
                    command.Parameters.AddWithValue("@Title", note.Title ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Content", note.Content ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedAt", note.UpdatedAt ?? (object)DBNull.Value);
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        
        public bool DeleteNote(int noteId)
        {
            try
            {
                string query = "DELETE FROM Notes WHERE Id = @Id";
                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Id", noteId);
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        
        #endregion
        
        #region Tag Operations
        
        public List<Tag> GetAllTags(int userId)
        {
            var tags = new List<Tag>();
            try
            {
                string query = "SELECT * FROM Tags WHERE UserId = @UserId";
                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tags.Add(new Tag
                            {
                                Id = reader.GetInt32("Id"),
                                Name = reader.IsDBNull("Name") ? string.Empty : reader.GetString("Name"),
                                Color = reader.IsDBNull("Color") ? string.Empty : reader.GetString("Color"),
                                UserId = reader.GetInt32("UserId"),
                                CreatedAt = reader.GetDateTime("CreatedAt")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return tags;
        }
        
        public int AddTag(Tag tag)
        {
            try
            {
                string query = "INSERT INTO Tags (Name, Color, UserId) VALUES (@Name, @Color, @UserId); SELECT SCOPE_IDENTITY()";
                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Name", tag.Name ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Color", tag.Color ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UserId", tag.UserId);
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        
        public bool UpdateTag(Tag tag)
        {
            try
            {
                string query = "UPDATE Tags SET Name = @Name, Color = @Color WHERE Id = @Id";
                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Id", tag.Id);
                    command.Parameters.AddWithValue("@Name", tag.Name ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Color", tag.Color ?? (object)DBNull.Value);
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        
        public bool DeleteTag(int tagId)
        {
            try
            {
                string query = "DELETE FROM Tags WHERE Id = @Id";
                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Id", tagId);
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        
        #endregion
        
        #region TaskTag Operations
        
        public List<TaskTag> GetTaskTags(int taskId)
        {
            var taskTags = new List<TaskTag>();
            try
            {
                string query = "SELECT * FROM TaskTags WHERE TaskId = @TaskId";
                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@TaskId", taskId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            taskTags.Add(new TaskTag
                            {
                                Id = reader.GetInt32("Id"),
                                TaskId = reader.GetInt32("TaskId"),
                                TagId = reader.GetInt32("TagId")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return taskTags;
        }
        
        public int AddTaskTag(TaskTag taskTag)
        {
            try
            {
                string query = "INSERT INTO TaskTags (TaskId, TagId) VALUES (@TaskId, @TagId); SELECT SCOPE_IDENTITY()";
                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@TaskId", taskTag.TaskId);
                    command.Parameters.AddWithValue("@TagId", taskTag.TagId);
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        
        public bool DeleteTaskTag(int taskTagId)
        {
            try
            {
                string query = "DELETE FROM TaskTags WHERE Id = @Id";
                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Id", taskTagId);
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        
        #endregion
        
        #region Reminder Operations
        
        public List<Reminder> GetActiveReminders()
        {
            var reminders = new List<Reminder>();
            try
            {
                string query = "SELECT r.*, t.Title as TaskTitle FROM Reminders r LEFT JOIN Tasks t ON r.TaskId = t.Id WHERE r.IsActive = 1 AND r.ReminderTime <= GETDATE()";
                using (var command = new SqlCommand(query, _connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reminders.Add(new Reminder
                            {
                                Id = reader.GetInt32("Id"),
                                TaskId = reader.GetInt32("TaskId"),
                                ReminderTime = reader.GetDateTime("ReminderTime"),
                                Message = reader.IsDBNull("Message") ? string.Empty : reader.GetString("Message"),
                                IsActive = reader.GetBoolean("IsActive"),
                                UserId = reader.GetInt32("UserId"),
                                CreatedAt = reader.GetDateTime("CreatedAt"),
                                Task = new Task { Title = reader.IsDBNull("TaskTitle") ? string.Empty : reader.GetString("TaskTitle") }
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return reminders;
        }
        
        public List<Reminder> GetRemindersByUser(int userId)
        {
            var reminders = new List<Reminder>();
            try
            {
                string query = "SELECT r.*, t.Title as TaskTitle FROM Reminders r LEFT JOIN Tasks t ON r.TaskId = t.Id WHERE r.UserId = @UserId";
                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reminders.Add(new Reminder
                            {
                                Id = reader.GetInt32("Id"),
                                TaskId = reader.GetInt32("TaskId"),
                                ReminderTime = reader.GetDateTime("ReminderTime"),
                                Message = reader.IsDBNull("Message") ? string.Empty : reader.GetString("Message"),
                                IsActive = reader.GetBoolean("IsActive"),
                                UserId = reader.GetInt32("UserId"),
                                CreatedAt = reader.GetDateTime("CreatedAt"),
                                Task = new Task { Title = reader.IsDBNull("TaskTitle") ? string.Empty : reader.GetString("TaskTitle") }
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return reminders;
        }
        
        public int AddReminder(Reminder reminder)
        {
            try
            {
                string query = "INSERT INTO Reminders (TaskId, ReminderTime, Message, IsActive, UserId) VALUES (@TaskId, @ReminderTime, @Message, @IsActive, @UserId); SELECT SCOPE_IDENTITY()";
                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@TaskId", reminder.TaskId);
                    command.Parameters.AddWithValue("@ReminderTime", reminder.ReminderTime);
                    command.Parameters.AddWithValue("@Message", reminder.Message ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IsActive", reminder.IsActive);
                    command.Parameters.AddWithValue("@UserId", reminder.UserId);
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        
        public bool UpdateReminder(Reminder reminder)
        {
            try
            {
                string query = "UPDATE Reminders SET TaskId = @TaskId, ReminderTime = @ReminderTime, Message = @Message, IsActive = @IsActive WHERE Id = @Id";
                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Id", reminder.Id);
                    command.Parameters.AddWithValue("@TaskId", reminder.TaskId);
                    command.Parameters.AddWithValue("@ReminderTime", reminder.ReminderTime);
                    command.Parameters.AddWithValue("@Message", reminder.Message ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IsActive", reminder.IsActive);
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        
        public bool DeactivateReminder(int reminderId)
        {
            try
            {
                string query = "UPDATE Reminders SET IsActive = 0 WHERE Id = @Id";
                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Id", reminderId);
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        
        public bool DeleteReminder(int reminderId)
        {
            try
            {
                string query = "DELETE FROM Reminders WHERE Id = @Id";
                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Id", reminderId);
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        
        #endregion
        
        #region Role and Gender Operations
        
        public List<Role> GetAllRoles()
        {
            var roles = new List<Role>();
            try
            {
                string query = "SELECT * FROM Roles";
                using (var command = new SqlCommand(query, _connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            roles.Add(new Role
                            {
                                Id = reader.GetInt32("Id"),
                                Name = reader.GetString("Name")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return roles;
        }
        
        public List<Gender> GetAllGenders()
        {
            var genders = new List<Gender>();
            try
            {
                string query = "SELECT * FROM Genders";
                using (var command = new SqlCommand(query, _connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            genders.Add(new Gender
                            {
                                Id = reader.GetInt32("Id"),
                                Name = reader.GetString("Name")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return genders;
        }
        
        #endregion
        
        #region Search and Filter Operations
        
        public List<Task> SearchTasks(int userId, string searchQuery)
        {
            var tasks = new List<Task>();
            try
            {
                string query = "SELECT * FROM Tasks WHERE UserId = @UserId AND (Title LIKE @SearchQuery OR Description LIKE @SearchQuery)";
                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@SearchQuery", "%" + searchQuery + "%");
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tasks.Add(new Task
                            {
                                Id = reader.GetInt32("Id"),
                                Title = reader.IsDBNull("Title") ? string.Empty : reader.GetString("Title"),
                                Description = reader.IsDBNull("Description") ? string.Empty : reader.GetString("Description"),
                                DueDate = reader.IsDBNull("DueDate") ? (DateTime?)null : reader.GetDateTime("DueDate"),
                                Priority = reader.IsDBNull("Priority") ? string.Empty : reader.GetString("Priority"),
                                Status = reader.IsDBNull("Status") ? string.Empty : reader.GetString("Status"),
                                UserId = reader.GetInt32("UserId"),
                                CreatedAt = reader.GetDateTime("CreatedAt")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return tasks;
        }
        
        public List<Task> FilterTasks(int userId, string status, string priority, DateTime? dueDate)
        {
            var tasks = new List<Task>();
            try
            {
                string query = "SELECT * FROM Tasks WHERE UserId = @UserId";
                var conditions = new List<string>();
                
                if (!string.IsNullOrEmpty(status))
                {
                    conditions.Add("Status = @Status");
                }
                if (!string.IsNullOrEmpty(priority))
                {
                    conditions.Add("Priority = @Priority");
                }
                if (dueDate.HasValue)
                {
                    conditions.Add("DueDate = @DueDate");
                }
                
                if (conditions.Count > 0)
                {
                    query += " AND " + string.Join(" AND ", conditions);
                }
                
                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    if (!string.IsNullOrEmpty(status))
                    {
                        command.Parameters.AddWithValue("@Status", status);
                    }
                    if (!string.IsNullOrEmpty(priority))
                    {
                        command.Parameters.AddWithValue("@Priority", priority);
                    }
                    if (dueDate.HasValue)
                    {
                        command.Parameters.AddWithValue("@DueDate", dueDate.Value);
                    }
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tasks.Add(new Task
                            {
                                Id = reader.GetInt32("Id"),
                                Title = reader.IsDBNull("Title") ? string.Empty : reader.GetString("Title"),
                                Description = reader.IsDBNull("Description") ? string.Empty : reader.GetString("Description"),
                                DueDate = reader.IsDBNull("DueDate") ? (DateTime?)null : reader.GetDateTime("DueDate"),
                                Priority = reader.IsDBNull("Priority") ? string.Empty : reader.GetString("Priority"),
                                Status = reader.IsDBNull("Status") ? string.Empty : reader.GetString("Status"),
                                UserId = reader.GetInt32("UserId"),
                                CreatedAt = reader.GetDateTime("CreatedAt")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return tasks;
        }
        
        #endregion
    }
}
