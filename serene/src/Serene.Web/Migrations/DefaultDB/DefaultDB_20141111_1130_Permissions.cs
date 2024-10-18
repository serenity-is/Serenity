using FluentMigrator;

namespace Serene.Migrations.DefaultDB;

[DefaultDB, MigrationKey(20141111_1130)]
public class DefaultDB_20141111_1130_Permissions : AutoReversingMigration
{
    public override void Up()
    {
        this.CreateTableWithId64("UserPermissions", "UserPermissionId", s => s
            .WithColumn("UserId").AsInt32().NotNullable()
                .ForeignKey("FK_UserPermissions_UserId", "Users", "UserId")
            .WithColumn("PermissionKey").AsString(100).NotNullable()
            .WithColumn("Granted").AsBoolean().NotNullable().WithDefaultValue(true));

        Create.Index("UQ_UserPerm_UserId_PermKey")
            .OnTable("UserPermissions")
            .OnColumn("UserId").Ascending()
            .OnColumn("PermissionKey").Ascending()
            .WithOptions().Unique();

        this.CreateTableWithId32("Roles", "RoleId", s => s
            .WithColumn("RoleName").AsString(100).NotNullable());

        this.CreateTableWithId64("RolePermissions", "RolePermissionId", s => s
            .WithColumn("RoleId").AsInt32().NotNullable()
                .ForeignKey("FK_RolePermissions_RoleId", "Roles", "RoleId")
            .WithColumn("PermissionKey").AsString(100).NotNullable());

        Create.Index("UQ_RolePerm_RoleId_PermKey")
            .OnTable("RolePermissions")
            .OnColumn("RoleId").Ascending()
            .OnColumn("PermissionKey").Ascending()
            .WithOptions().Unique();

        this.CreateTableWithId64("UserRoles", "UserRoleId", s => s
            .WithColumn("UserId").AsInt32().NotNullable()
                .ForeignKey("FK_UserRoles_UserId", "Users", "UserId")
            .WithColumn("RoleId").AsInt32().NotNullable()
                .ForeignKey("FK_UserRoles_RoleId", "Roles", "RoleId"));

        Create.Index("UQ_UserRoles_UserId_RoleId")
            .OnTable("UserRoles")
            .OnColumn("UserId").Ascending()
            .OnColumn("RoleId").Ascending()
            .WithOptions().Unique();

        Create.Index("IX_UserRoles_RoleId_UserId")
            .OnTable("UserRoles")
            .OnColumn("RoleId").Ascending()
            .OnColumn("UserId").Ascending();
    }
}