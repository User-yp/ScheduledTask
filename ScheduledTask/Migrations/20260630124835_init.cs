using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduledTask.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "JOB_CONFIG",
                columns: table => new
                {
                    GROUP = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    JOB_KEYNAME = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    JOB_DESC = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TRIGGER_KEYNAME = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TRIGGER_DESC = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CRON = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CRON_DESC = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IS_ENABLE = table.Column<string>(type: "varchar(1)", maxLength: 1, nullable: false, defaultValue: "N")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JOB_CONFIG", x => new { x.GROUP, x.JOB_KEYNAME });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "qrtz_blob_triggers",
                columns: table => new
                {
                    SCHED_NAME = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TRIGGER_NAME = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TRIGGER_GROUP = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BLOB_DATA = table.Column<byte[]>(type: "longblob", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qrtz_blob_triggers", x => new { x.SCHED_NAME, x.TRIGGER_NAME, x.TRIGGER_GROUP });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "qrtz_calendars",
                columns: table => new
                {
                    sched_name = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    calendar_name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    calendar = table.Column<byte[]>(type: "longblob", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qrtz_calendars", x => new { x.sched_name, x.calendar_name });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "qrtz_cron_triggers",
                columns: table => new
                {
                    sched_name = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    trigger_name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    trigger_group = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cron_expression = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    time_zone_id = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qrtz_cron_triggers", x => new { x.sched_name, x.trigger_name, x.trigger_group });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "qrtz_fired_triggers",
                columns: table => new
                {
                    sched_name = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    entry_id = table.Column<string>(type: "varchar(95)", maxLength: 95, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    trigger_name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    trigger_group = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    instance_name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fired_time = table.Column<long>(type: "bigint", nullable: false),
                    sched_time = table.Column<long>(type: "bigint", nullable: false),
                    priority = table.Column<int>(type: "int", nullable: false),
                    state = table.Column<string>(type: "varchar(16)", maxLength: 16, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    job_name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    job_group = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_nonconcurrent = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    requests_recovery = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qrtz_fired_triggers", x => new { x.sched_name, x.entry_id, x.trigger_name, x.trigger_group });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "qrtz_job_details",
                columns: table => new
                {
                    sched_name = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    job_name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    job_group = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    job_class_name = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_durable = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_nonconcurrent = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_update_data = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    requests_recovery = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    job_data = table.Column<byte[]>(type: "longblob", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qrtz_job_details", x => new { x.sched_name, x.job_name, x.job_group });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "qrtz_locks",
                columns: table => new
                {
                    sched_name = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    lock_name = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qrtz_locks", x => new { x.sched_name, x.lock_name });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "qrtz_paused_trigger_grps",
                columns: table => new
                {
                    sched_name = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    trigger_group = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qrtz_paused_trigger_grps", x => new { x.sched_name, x.trigger_group });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "qrtz_scheduler_state",
                columns: table => new
                {
                    sched_name = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    instance_name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_checkin_time = table.Column<long>(type: "bigint", nullable: false),
                    checkin_interval = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qrtz_scheduler_state", x => new { x.sched_name, x.instance_name });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "qrtz_simple_triggers",
                columns: table => new
                {
                    sched_name = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    trigger_name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    trigger_group = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    repeat_count = table.Column<long>(type: "bigint", nullable: false),
                    repeat_interval = table.Column<long>(type: "bigint", nullable: false),
                    times_triggered = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qrtz_simple_triggers", x => new { x.sched_name, x.trigger_name, x.trigger_group });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "qrtz_simprop_triggers",
                columns: table => new
                {
                    sched_name = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    trigger_name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    trigger_group = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    str_prop_1 = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    str_prop_2 = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    str_prop_3 = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    int_prop_1 = table.Column<int>(type: "int", nullable: true),
                    int_prop_2 = table.Column<int>(type: "int", nullable: true),
                    long_prop_1 = table.Column<long>(type: "bigint", nullable: true),
                    long_prop_2 = table.Column<long>(type: "bigint", nullable: true),
                    dec_prop_1 = table.Column<decimal>(type: "numeric(13,4)", nullable: true),
                    dec_prop_2 = table.Column<decimal>(type: "numeric(13,4)", nullable: true),
                    bool_prop_1 = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    bool_prop_2 = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    time_zone_id = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qrtz_simprop_triggers", x => new { x.sched_name, x.trigger_name, x.trigger_group });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "qrtz_triggers",
                columns: table => new
                {
                    sched_name = table.Column<string>(type: "varchar(120)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    trigger_name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    trigger_group = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    job_name = table.Column<string>(type: "varchar(150)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    job_group = table.Column<string>(type: "varchar(150)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    next_fire_time = table.Column<long>(type: "bigint", nullable: true),
                    prev_fire_time = table.Column<long>(type: "bigint", nullable: true),
                    priority = table.Column<int>(type: "int", nullable: true),
                    trigger_state = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    trigger_type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    start_time = table.Column<long>(type: "bigint", nullable: false),
                    end_time = table.Column<long>(type: "bigint", nullable: true),
                    calendar_name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    misfire_instr = table.Column<short>(type: "smallint", nullable: true),
                    job_data = table.Column<byte[]>(type: "longblob", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qrtz_triggers", x => new { x.sched_name, x.trigger_name, x.trigger_group });
                    table.ForeignKey(
                        name: "FK_TRIGGERS_JOB_DETAILS",
                        columns: x => new { x.sched_name, x.job_name, x.job_group },
                        principalTable: "qrtz_job_details",
                        principalColumns: new[] { "sched_name", "job_name", "job_group" },
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_JOB_CONFIG_GROUP",
                table: "JOB_CONFIG",
                column: "GROUP");

            migrationBuilder.CreateIndex(
                name: "IX_JOB_CONFIG_JOB_KEYNAME",
                table: "JOB_CONFIG",
                column: "JOB_KEYNAME");

            migrationBuilder.CreateIndex(
                name: "IX_qrtz_triggers_sched_name_job_name_job_group",
                table: "qrtz_triggers",
                columns: new[] { "sched_name", "job_name", "job_group" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JOB_CONFIG");

            migrationBuilder.DropTable(
                name: "qrtz_blob_triggers");

            migrationBuilder.DropTable(
                name: "qrtz_calendars");

            migrationBuilder.DropTable(
                name: "qrtz_cron_triggers");

            migrationBuilder.DropTable(
                name: "qrtz_fired_triggers");

            migrationBuilder.DropTable(
                name: "qrtz_locks");

            migrationBuilder.DropTable(
                name: "qrtz_paused_trigger_grps");

            migrationBuilder.DropTable(
                name: "qrtz_scheduler_state");

            migrationBuilder.DropTable(
                name: "qrtz_simple_triggers");

            migrationBuilder.DropTable(
                name: "qrtz_simprop_triggers");

            migrationBuilder.DropTable(
                name: "qrtz_triggers");

            migrationBuilder.DropTable(
                name: "qrtz_job_details");
        }
    }
}
