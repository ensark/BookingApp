using System;
using GeoAPI.Geometries;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Booking.Infrastructure.Database.Migrations
{
    public partial class InitialDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    ModifiedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 256, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 256, nullable: true),
                    FirstName = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    PasswordHash = table.Column<byte[]>(nullable: false),
                    PasswordSalt = table.Column<byte[]>(nullable: false),
                    Phone = table.Column<string>(nullable: false),
                    UserTypeId = table.Column<int>(nullable: false),
                    Biography = table.Column<string>(maxLength: 5000, nullable: true),
                    Role = table.Column<string>(nullable: false),
                    FcmTokenDeviceId = table.Column<string>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    ModifiedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 256, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 256, nullable: true),
                    Street = table.Column<string>(nullable: false),
                    City = table.Column<string>(nullable: false),
                    Postcode = table.Column<string>(nullable: false),
                    Country = table.Column<string>(nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Addresses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Attachments",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    ModifiedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 256, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 256, nullable: true),
                    FileName = table.Column<string>(nullable: false),
                    DocumentTypeId = table.Column<int>(nullable: false),
                    ContentType = table.Column<string>(nullable: false),
                    Data = table.Column<byte[]>(nullable: false),
                    UserId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attachments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    ModifiedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 256, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 256, nullable: true),
                    SenderId = table.Column<long>(nullable: false),
                    ReceiverId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chats_Users_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Chats_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Connections",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    ModifiedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 256, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 256, nullable: true),
                    CustomerId = table.Column<long>(nullable: false),
                    ProviderId = table.Column<long>(nullable: false),
                    CustomerSentRequest = table.Column<bool>(nullable: false),
                    ProviderSentRequest = table.Column<bool>(nullable: false),
                    ConnectionStatusId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Connections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Connections_Users_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Connections_Users_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "HubConnections",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    ModifiedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 256, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 256, nullable: true),
                    ConnectionId = table.Column<string>(maxLength: 256, nullable: false),
                    UserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HubConnections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HubConnections_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Invites",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    ModifiedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 256, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 256, nullable: true),
                    InviterId = table.Column<long>(nullable: false),
                    FriendNumber = table.Column<string>(nullable: false),
                    VoucherCodeSent = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invites_Users_InviterId",
                        column: x => x.InviterId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    ModifiedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 256, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 256, nullable: true),
                    SenderId = table.Column<long>(nullable: false),
                    ReceiverId = table.Column<long>(nullable: false),
                    Title = table.Column<string>(maxLength: 5000, nullable: false),
                    Content = table.Column<string>(nullable: false),
                    NotificationStatusId = table.Column<int>(nullable: false),
                    NotificationTypeId = table.Column<int>(nullable: false),
                    NotificationSentAt = table.Column<DateTime>(nullable: false),
                    ReservationId = table.Column<long>(nullable: true),
                    ConnectionId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "NotificationSettings",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    ModifiedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 256, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 256, nullable: true),
                    BookingConfirmations = table.Column<bool>(nullable: false),
                    RecommendationRequestFromFriends = table.Column<bool>(nullable: false),
                    PrivateMessages = table.Column<bool>(nullable: false),
                    NewBookings = table.Column<bool>(nullable: false),
                    AutomaticBookingConfirmation = table.Column<bool>(nullable: false),
                    NotificationSettingsTypeId = table.Column<int>(nullable: false),
                    UserId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationSettings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Providers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    ModifiedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 256, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 256, nullable: true),
                    Title = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    ServiceTypeId = table.Column<int>(nullable: false),
                    ProfessionTypeId = table.Column<int>(nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    NumberOfParticipants = table.Column<int>(nullable: true),
                    FiveSessionsDiscount = table.Column<float>(nullable: true),
                    TenSessionsDiscount = table.Column<float>(nullable: true),
                    ProviderStrypeId = table.Column<string>(nullable: true),
                    UserId = table.Column<long>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Providers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Providers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProviderSkills",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    ModifiedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 256, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 256, nullable: true),
                    SkillName = table.Column<string>(nullable: false),
                    UserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderSkills_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    ModifiedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 256, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 256, nullable: true),
                    Token = table.Column<string>(nullable: false),
                    Role = table.Column<string>(nullable: false),
                    UserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reminders",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    ModifiedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 256, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 256, nullable: true),
                    Booking24HoursBefore = table.Column<bool>(nullable: false),
                    Booking1HourBefore = table.Column<bool>(nullable: false),
                    Booking15MinutesBefore = table.Column<bool>(nullable: false),
                    UserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reminders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reminders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    ModifiedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 256, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 256, nullable: true),
                    Grade = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    PostDate = table.Column<DateTime>(nullable: false),
                    Comment = table.Column<string>(maxLength: 1000, nullable: false),
                    RatedUserId = table.Column<long>(nullable: false),
                    ReviewerId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_RatedUserId",
                        column: x => x.RatedUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "VoucherCodes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    ModifiedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 256, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 256, nullable: true),
                    Code = table.Column<string>(nullable: false),
                    IsUsed = table.Column<bool>(nullable: false),
                    UserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoucherCodes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    ModifiedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 256, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 256, nullable: true),
                    SenderName = table.Column<string>(nullable: false),
                    ReceiverName = table.Column<string>(nullable: false),
                    Content = table.Column<string>(maxLength: 5000, nullable: false),
                    MessageSentAt = table.Column<DateTime>(nullable: false),
                    IsSend = table.Column<bool>(nullable: false),
                    IsRead = table.Column<bool>(nullable: false),
                    ChatId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    ModifiedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 256, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 256, nullable: true),
                    Name = table.Column<string>(nullable: false),
                    GeoLocation = table.Column<IPoint>(nullable: false),
                    ProviderId = table.Column<long>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locations_Providers_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    ModifiedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 256, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 256, nullable: true),
                    ReservationStatusId = table.Column<int>(nullable: false),
                    PaymentProviderId = table.Column<int>(nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    TotalPriceDiscount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true),
                    FiveSessionsDiscount = table.Column<float>(nullable: true),
                    TenSessionsDiscount = table.Column<float>(nullable: true),
                    VoucherCodeDiscount = table.Column<float>(nullable: true),
                    VoucherCode = table.Column<string>(nullable: true),
                    PayTotal = table.Column<bool>(nullable: false),
                    PayPerSession = table.Column<bool>(nullable: false),
                    ProviderId = table.Column<long>(nullable: false),
                    CustomerId = table.Column<long>(nullable: false),
                    ReservationPaymentId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservations_Users_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservations_Providers_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleSettings",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    ModifiedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 256, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 256, nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    WorkingHoursStart = table.Column<string>(nullable: true),
                    WorkingHoursEnd = table.Column<string>(nullable: true),
                    DurationOfSessionInMinutes = table.Column<int>(nullable: false),
                    GapBetweenSessionsInMinutes = table.Column<int>(nullable: false),
                    ScheduledDaysOfWeek = table.Column<string>(nullable: true),
                    ScheduledTimeSlots = table.Column<string>(nullable: true),
                    ProviderId = table.Column<long>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduleSettings_Providers_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    ModifiedDateUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 256, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 256, nullable: true),
                    AppointmentTime = table.Column<DateTime>(nullable: false),
                    AppointmentStatusId = table.Column<int>(nullable: false),
                    PricePerSession = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    PricePerSessionDiscount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true),
                    ReservationId = table.Column<long>(nullable: false),
                    AppointmentExternalId = table.Column<Guid>(nullable: false),
                    PayPalApprovalLink = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointments_Reservations_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_UserId",
                table: "Addresses",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ReservationId",
                table: "Appointments",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_UserId",
                table: "Attachments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ChatId",
                table: "ChatMessages",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_ReceiverId",
                table: "Chats",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_SenderId",
                table: "Chats",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Connections_CustomerId",
                table: "Connections",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Connections_ProviderId",
                table: "Connections",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_HubConnections_UserId",
                table: "HubConnections",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Invites_InviterId",
                table: "Invites",
                column: "InviterId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_ProviderId",
                table: "Locations",
                column: "ProviderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ReceiverId",
                table: "Notifications",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_SenderId",
                table: "Notifications",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationSettings_UserId",
                table: "NotificationSettings",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Providers_UserId",
                table: "Providers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderSkills_UserId",
                table: "ProviderSkills",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_UserId",
                table: "Reminders",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_CustomerId",
                table: "Reservations",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ProviderId",
                table: "Reservations",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_RatedUserId",
                table: "Reviews",
                column: "RatedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ReviewerId",
                table: "Reviews",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleSettings_ProviderId",
                table: "ScheduleSettings",
                column: "ProviderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VoucherCodes_UserId",
                table: "VoucherCodes",
                column: "UserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "Attachments");

            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "Connections");

            migrationBuilder.DropTable(
                name: "HubConnections");

            migrationBuilder.DropTable(
                name: "Invites");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "NotificationSettings");

            migrationBuilder.DropTable(
                name: "ProviderSkills");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "Reminders");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "ScheduleSettings");

            migrationBuilder.DropTable(
                name: "VoucherCodes");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Chats");

            migrationBuilder.DropTable(
                name: "Providers");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
