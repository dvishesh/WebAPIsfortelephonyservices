using System;
using Webserver.PerformCallModels;
using Webserver.SendFaxModels;
using Webserver.SendUpdateModels;
using Webserver.EnquireAwbInboundModels;
using Microsoft.EntityFrameworkCore;
 
namespace Webserver.Contexts
{
   
    public class PerformCallDbInjection : DbContext {
    public PerformCallDbInjection(DbContextOptions<PerformCallDbInjection> options)
    : base(options) { }

    public DbSet<Sats_NotificationRequest_Details> PrimaryTableDBObject { get; set; }
    public DbSet<Sats_Outbound_CallNotification_Records> SecondaryTableDBObject {get;set;}
    public DbSet<Sats_ConsignmentParts_Details> TertiaryTableDBObject {get; set;}
    // DbSet<T> type properties for other domain models
    }

public class PerformCallDbContextFactory {
    public static PerformCallDbInjection Create(string connectionString) {
        var optionsBuilder = new DbContextOptionsBuilder<PerformCallDbInjection>();
        optionsBuilder.UseSqlServer(connectionString);
        var performcalldbinjection = new PerformCallDbInjection(optionsBuilder.Options);
        return performcalldbinjection;
        }

    }

    public class SendFaxDbInjection : DbContext {
    public SendFaxDbInjection(DbContextOptions<SendFaxDbInjection> options)
    : base(options) { }

    public DbSet<RequestMain_Details> PrimaryTableDBObject { get; set; }
    public DbSet<PhNoDetails> SecondaryTableDBObject {get;set;}
    public DbSet<AwbDetails> TertiaryTableDBObject {get; set;}
    // DbSet<T> type properties for other domain models
    }

public class SendFaxDbContextFactory {
    public static SendFaxDbInjection Create(string connectionString) {
        var optionsBuilder = new DbContextOptionsBuilder<SendFaxDbInjection>();
        optionsBuilder.UseSqlServer(connectionString);
        var sendfaxdbinjection = new SendFaxDbInjection(optionsBuilder.Options);
        return sendfaxdbinjection;
        }

    }


    
    public class SendUpdateDbContext : DbContext {
    public SendUpdateDbContext(DbContextOptions<SendUpdateDbContext> options)
    : base(options) { }

    public DbSet<SendUpdate_Details_Call> CallDBObject { get; set; }
    public DbSet<SendUpdate_Details_Fax> FaxDBObject { get; set; }
    // DbSet<T> type properties for other domain models
    }

public class SendUpdateDbContextFactory {
    public static SendUpdateDbContext Create(string connectionString) {
        var optionsBuilder = new DbContextOptionsBuilder<SendUpdateDbContext>();
        optionsBuilder.UseSqlServer(connectionString);
        var sendupdatedbcontext = new SendUpdateDbContext(optionsBuilder.Options);
        return sendupdatedbcontext;
        }
    }

    public class EnquireAwbDbInjection : DbContext {
    public EnquireAwbDbInjection(DbContextOptions<EnquireAwbDbInjection> options)
    : base(options) { }

    public DbSet<AwbEnquireRequest_MainDetails> PrimaryTableDBObject { get; set; }
    public DbSet<AwbEnquireRequest_ConsignmentParts_Details> SecondaryTableDBObject { get; set; }
    // DbSet<T> type properties for other domain models
    }

public class EnquireAwbDbContextFactory {
    public static EnquireAwbDbInjection Create(string connectionString) {
        var optionsBuilder = new DbContextOptionsBuilder<EnquireAwbDbInjection>();
        optionsBuilder.UseSqlServer(connectionString);
        var awbenquiredbcontext = new EnquireAwbDbInjection(optionsBuilder.Options);
        return awbenquiredbcontext;
        }
    }
}