using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text;
using System.Windows;

public class User
{
    public int UserID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public bool IsAdmin { get; set; }
    public List<Post> Posts { get; set; } = new();
    public  List<Friend> Friends { get; set; } = new();
    public  List<UserGroup> UserGroups { get; set; } = new(); // Добавлено для связи с группами
    public  List<Message> SentMessages { get; set; } = new(); // Добавлено для связи с отправленными сообщениями
    public  List<Message> ReceivedMessages { get; set; } = new();// Добавлено для связи с полученными сообщениями
    public  List<UserEvent> UserEvents { get; set; } = new();// Добавлено для связи с событиями
    public  List<Notification> Notifications { get; set; } = new();// Добавлено для связи с уведомлениями
    public  List<Comment> Comments { get; set; } = new();// Добавлено для связи с уведомлениями
}

public class Post
{
    public int PostID { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public int UserID { get; set; }
    public User? User { get; set; }
    public virtual List<Comment> Comments { get; set; } = new();
}

public class Comment
{
    public int CommentID { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public int PostID { get; set; }
    public int UserID { get; set; }
    
    public Post? Post { get; set; }
    public User? User {  get; set; }
}

public class Friend
{
    public int FriendID { get; set; }
    public int UserID { get; set; }
    public int FriendUserID { get; set; }
    public User? User { get; set; }
    public User? FriendUser { get; set; } = new();
}

public class Group
{
    public int GroupID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<UserGroup> UserGroups { get; set; } = new();
}

public class UserGroup
{
    public int UserGroupID { get; set; }
    public int UserID { get; set; }
    public int GroupID { get; set; }
    public User? User { get; set; }
    public Group? Group { get; set; }
}

public class Message
{
    public int MessageID { get; set; }
    public string Content { get; set; }
    public DateTime SentAt { get; set; }

    public int FromUserID { get; set; } // Foreign key for the sender
    public User? FromUser { get; set; } // Navigation property for the sender

    public int ToUserID { get; set; } // Foreign key for the recipient
    public User? ToUser { get; set; } // Navigation property for the recipient
}

public class Event
{
    public int EventID { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime Time { get; set; }
    public List<UserEvent> UserEvents { get; set; }
}

public class UserEvent
{
    public int UserEventID { get; set; }
    public int EventID { get; set; }
    public int UserID { get; set; }
    public bool IsGoing { get; set; }
    public Event? Event { get; set; }
    public User? User { get; set; }
}

public class Notification
{
    public int NotificationID { get; set; }
    public string Content { get; set; }
    public int UserID { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public User? User { get; set; }
}

public class YourDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Friend> Friends { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<UserGroup> UserGroups { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<UserEvent> UserEvents { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    public YourDbContext()
    {
        Database.EnsureCreated();
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Здесь необходимо указать строку подключения к вашей базе данных
        optionsBuilder.UseSqlServer("Server=localhost,1434;Database=db1;User Id=sa;Password=xxXX123!;TrustServerCertificate=true;");
        optionsBuilder.UseSqlServer("Server=localhost,1434;Database=db1;User Id=sa;Password=xxXX123!;TrustServerCertificate=true;", options =>
        {
            options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
        });

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Настройки моделей (примеры индексов, ограничений и т.д.) могут быть добавлены здесь
        // Например:
        // modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

        // Определяем связи между моделями
        

        modelBuilder.Entity<User>().ToTable("User");
        modelBuilder.Entity<Post>().ToTable("Post");
        modelBuilder.Entity<Comment>().ToTable("Comment");
        modelBuilder.Entity<Friend>().ToTable("Friend");
        modelBuilder.Entity<Group>().ToTable("Group");
        modelBuilder.Entity<UserGroup>().ToTable("UserGroup");
        modelBuilder.Entity<Message>().ToTable("Message");
        modelBuilder.Entity<Event>().ToTable("Event");
        modelBuilder.Entity<UserEvent>().ToTable("UserEvent");
        modelBuilder.Entity<Notification>().ToTable("Notification");

        

        modelBuilder.Entity<Friend>()
            .HasOne(f => f.User)
        .WithMany(u => u.Friends)
        .HasForeignKey(f => f.UserID)
        .OnDelete(DeleteBehavior.Restrict); // Adjust delete behavior as necessary

        modelBuilder.Entity<Friend>()
            .HasOne(f => f.FriendUser)
            .WithMany() // If you want to access the reverse relation, you might need another collection property
            .HasForeignKey(f => f.FriendUserID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>()
        .HasOne(m => m.FromUser) // Navigation property in Message
        .WithMany(u => u.SentMessages) // Corresponding collection in User
        .HasForeignKey(m => m.FromUserID) // Foreign key in Message
        .OnDelete(DeleteBehavior.Restrict); // Optional: Adjust delete behavior as necessary

        modelBuilder.Entity<Message>()
            .HasOne(m => m.ToUser) // Navigation property in Message
            .WithMany(u => u.ReceivedMessages) // Corresponding collection in User
            .HasForeignKey(m => m.ToUserID) // Foreign key in Message
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Comment>()
        .HasOne(c => c.User) // Navigation property in Comment
        .WithMany(u => u.Comments) // Corresponding collection in User
        .HasForeignKey(c => c.UserID) // Foreign key in Comment
        .OnDelete(DeleteBehavior.NoAction);
    }
}

public class DataClassExecuteSQL
{
    public bool status;
    public string? message;

    public DataClassExecuteSQL(bool status, string? message)
    {
        this.status = status;
        this.message = message;
    }
}

public class DataBaseUtils
{
    public List<object> GetAllByTableName(DbContext context, string tableName)
    {
        var properties = context.GetType().GetProperties()
            .Where(p => p.PropertyType.IsGenericType &&
                        p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

        foreach (var prop in properties)
        {
            if (prop.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase))
            {
                var dbSet = prop.GetValue(context);
                var localData = ((IEnumerable<object>)dbSet).ToList();
                return localData;
            }
        }

        throw new ArgumentException("Таблица с таким названием не найдена: " + tableName);
    }

    public void DeleteRecordById(DbContext context, string tableName, int recordId)
    {
        // Получаем свойства контекста, которые являются DbSet<T>
        var properties = context.GetType().GetProperties()
            .Where(p => p.PropertyType.IsGenericType &&
                        p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

        foreach (var prop in properties)
        {
            if (prop.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase))
            {
                // Находим DbSet<T>, соответствующий указанной таблице
                var dbSet = prop.GetValue(context);

                // Получаем метод Find у DbSet<T> и вызываем его с переданным идентификатором записи
                var findMethod = dbSet.GetType().GetMethod("Find", new Type[] { typeof(object[]) });
                var entity = findMethod.Invoke(dbSet, new object[] { new object[] { recordId } });

                // Если запись найдена, удаляем её
                if (entity != null)
                {
                    // Получаем метод Remove у DbSet<T> и вызываем его с найденной сущностью
                    var removeMethod = dbSet.GetType().GetMethod("Remove", new Type[] { entity.GetType() });
                    removeMethod.Invoke(dbSet, new object[] { entity });

                    // Сохраняем изменения в базе данных
                    context.SaveChanges();
                    return;
                }
                else
                {
                    throw new ArgumentException($"Запись с идентификатором {recordId} не найдена в таблице {tableName}.");
                }
            }
        }

        throw new ArgumentException("Таблица с таким названием не найдена: " + tableName);
    }

    public List<string?> GetAllTables(DbContext context)
    {
        var model = context.Model;
        var tables = model.GetEntityTypes()
                          .Select(t => t.GetTableName())
                          .Distinct()
                          .ToList();
        return tables;
    }
    public Dictionary<string, Type> GetTableColumns(string tableName)
    {
        // Получаем тип таблицы из контекста
        var entityType = typeof(YourDbContext).GetProperties()
            .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
            .Select(p => p.PropertyType.GetGenericArguments().First())
            .FirstOrDefault(t => t.Name == tableName);

        if (entityType == null)
        {
            MessageBox.Show($"Таблица с названием {tableName} не найдена в контексте.");
        }

        // Получаем свойства типа, являющиеся столбцами таблицы
        var properties = entityType.GetProperties()
            .Where(p => p.GetCustomAttribute<ColumnAttribute>() != null || p.GetCustomAttribute<NotMappedAttribute>() == null);

        // Создаем словарь для хранения названий столбцов и их типов данных
        var columnTypes = new Dictionary<string, Type>();

        // Заполняем словарь
        foreach (var property in properties)
        {
            columnTypes[property.Name] = property.PropertyType;
        }

        return columnTypes;
    }

    public string BuildInsertQuery(string tableName, List<string> fields, List<object> values)
    {
        MessageBox.Show(String.Join(',', values));
        MessageBox.Show(String.Join(',', fields));
        if (fields.Count != values.Count)
        {
            MessageBox.Show("Количество полей и значений не совпадает.");
        }

        StringBuilder queryBuilder = new StringBuilder();
        queryBuilder.Append($"INSERT INTO [{tableName}] (");

        // Добавляем имена полей в запрос
        queryBuilder.Append(string.Join(", ", fields));
        queryBuilder.Append(") VALUES (");

        // Добавляем значения
        for (int i = 0; i < values.Count; i++)
        {
            // Проверяем тип значения и соответственно добавляем в запрос
            if (IsString(values[i]))
            {
                queryBuilder.Append($"'{values[i]}'");
            }
            else if (values[i] is bool)
            {
                queryBuilder.Append((bool)values[i] ? "1" : "0");
            }
            else if (values[i] is DateTime)
            {
                queryBuilder.Append($"'{((DateTime)values[i]).ToString("yyyy-MM-dd HH:mm:ss")}'");
            }
            else
            {
                queryBuilder.Append(values[i]);
            }

            // Добавляем запятую, если это не последнее значение
            if (i < values.Count - 1)
            {
                queryBuilder.Append(", ");
            }
        }

        queryBuilder.Append(");");

        return queryBuilder.ToString();
    }

    private static bool IsString(object value)
    {
        // Проверяем, является ли значение строкой
        return value is string;
    }

    public bool ExecuteSql(DbContext context, string sql)
    {
        try
        {
            context.Database.ExecuteSqlRaw(sql);
            return true;
        }
        catch (DbUpdateException ex)
        {
            MessageBox.Show($"DbUpdateException: {ex.Message}");
            return false;
        }
        catch (SqlException ex)
        {
            MessageBox.Show($"Ты что-то напутал с внешними ключами!");
            
            return false;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error executing SQL command: {ex.Message}");
            return false;
        }
    }

    public List<object> GetRecordValuesById(string tableName, int recordId)
    {
        Dictionary<string, Type> cols = GetTableColumns(tableName);
       
        // Создаем подключение к базе данных
        using (SqlConnection connection = new SqlConnection("Server=localhost,1434;Database=db1;User Id=sa;Password=xxXX123!;TrustServerCertificate=true;"))
        {
            // Открываем подключение
            connection.Open();

            // Создаем SQL-запрос
            string sqlQuery = $"SELECT * FROM [{tableName}] WHERE {cols.First().Key} = @RecordId;";
            
            // Создаем команду для выполнения запроса
            using (SqlCommand command = new SqlCommand(sqlQuery, connection))
            {
                // Добавляем параметр для передачи значения recordId
                command.Parameters.AddWithValue("@RecordId", recordId);

                // Выполняем запрос и получаем результаты
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    // Формируем сообщение с результатами запроса
                    var message = new StringBuilder();
                    List<object> values = new List<object>();
                    while (reader.Read())
                    {
                        // Читаем данные из результата запроса
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            message.Append(reader[i].ToString() + " ");
                            values.Add(reader[i]);
                    }
                        message.AppendLine();
                    }

                    // Вывод результатов через MessageBox
                    return values;
                }
            }
        }
    }

    public void UpdateRecord(DbContext context, string tableName, int recordId, List<string> fields, List<object> values, List<Type> tableColumnsWithType)
    {
        
        
        if (fields.Count != values.Count)
        {
            MessageBox.Show("Количество полей и значений не совпадает.");
        }

        // Получаем свойства контекста, которые являются DbSet<T>
        var properties = context.GetType().GetProperties()
            .Where(p => p.PropertyType.IsGenericType &&
                        p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

        foreach (var prop in properties)
        {
            if (prop.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase))
            {
                // Находим DbSet<T>, соответствующий указанной таблице
                var dbSet = prop.GetValue(context);

                // Получаем метод Find у DbSet<T> и вызываем его с переданным идентификатором записи
                var findMethod = dbSet.GetType().GetMethod("Find", new Type[] { typeof(object[]) });
                var entity = findMethod.Invoke(dbSet, new object[] { new object[] { recordId } });

                // Если запись найдена, обновляем её
                if (entity != null)
                {
                    // Получаем свойства сущности и обновляем их значения
                    var entityProperties = entity.GetType().GetProperties();
                    for (int i = 0; i < fields.Count; i++)
                    {
                        var property = entityProperties.FirstOrDefault(p => p.Name.Equals(fields[i], StringComparison.OrdinalIgnoreCase));
                        if (property != null)
                        {

                            if (tableColumnsWithType[i] == typeof(int))
                            {
                                int value = int.Parse(values[i].ToString());
                                property.SetValue(entity, value);

                            }
                            else if (tableColumnsWithType[i] == typeof(DateTime))
                            {
                                
                                property.SetValue(entity, values[i]);

                            }
                            else if(tableColumnsWithType[i] == typeof(bool))
                            {
                                bool value;
                                if (bool.Parse(values[i].ToString()) == true) { value = true; }
                                else { value = false; }
                                property.SetValue(entity, value);

                            }
                            else
                            {
                                var value = values[i];
                                property.SetValue(entity, value);

                            }
                        }
                        else
                        {
                            MessageBox.Show($"Столбец с именем {fields[i]} не найден в таблице {tableName}.");
                        }
                    }

                    // Сохраняем изменения в базе данных
                    context.SaveChanges();
                    return;
                }
                else
                {
                    MessageBox.Show($"Запись с идентификатором {recordId} не найдена в таблице {tableName}.");
                }
            }
        }

        MessageBox.Show("Таблица с таким названием не найдена: " + tableName);
    }

    public List<Message> GetMessagesForUserByDate(YourDbContext context, int userId, DateTime date)
    {
        try
        {
            // Выполняем запрос, чтобы получить все сообщения пользователя за определенную дату
            return context.Messages
                .Where(m => (m.FromUserID == userId || m.ToUserID == userId) && m.SentAt.Date == date.Date)
                .ToList();
        }
        catch (Exception ex)
        {
            // Обработка исключений, если что-то пошло не так
            Console.WriteLine($"Ошибка при получении сообщений: {ex.Message}");
            return new List<Message>(); // Возвращаем пустой список в случае ошибки
        }
    }
    public List<User?> GetCommonFriends(YourDbContext context, int userId1, int userId2)
    {
        var friends1 = context.Friends.Where(f => f.UserID == userId1).Select(f => f.FriendUserID);
        var friends2 = context.Friends.Where(f => f.UserID == userId2).Select(f => f.FriendUserID);

        // Находим общих друзей
        var commonFriendsIds = friends1.Intersect(friends2);

        // Получаем объекты пользователей из базы данных по их идентификаторам
        var commonFriends = context.Users.Where(u => commonFriendsIds.Contains(u.UserID)).ToList();

        return commonFriends;
    }
}
