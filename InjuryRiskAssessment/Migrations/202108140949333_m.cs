namespace InjuryRiskAssessment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "RegistrationDate", c => c.DateTime());
            AlterColumn("dbo.AspNetUsers", "LastLoginTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "LastLoginTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.AspNetUsers", "RegistrationDate", c => c.DateTime(nullable: false));
        }
    }
}
