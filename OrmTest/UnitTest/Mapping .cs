﻿using OrmTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.UnitTest
{
    public class Mapping:UnitTestBase
    {
        private Mapping() { }
        public Mapping(int eachCount)
        {
            this.Count = eachCount;
        }

        public void Init() {

            var db = GetInstance();
            var t1= db.Queryable<Student>().Where(it=>it.Id==1).ToSql();
            base.Check("SELECT [ID],[SchoolId],[Name],[CreateTime] FROM [STudent]  WHERE ( [ID] = @Id0 ) ", null, t1.Key, null,"Mapping t1 error");

            db.MappingColumns.Add("Id", "id", "School");
            var t2 = db.Queryable<Student, School>((st, sc) => new object[] {
                JoinType.Left,st.SchoolId==sc.Id
            })
            .Where(st => st.Id == 1)
            .Where<School>(sc => sc.Id == 1)
            .Where<School, Student>((sc, st) => sc.Id == st.Id)
            .GroupBy(st => st.Id)
            .GroupBy<School>(sc => sc.Id).OrderBy<Student>(st => st.Id,OrderByType.Asc)
            .Select<Student,School,dynamic>((st,sc)=> new { stid=st.Id,scid=sc.Id}).ToSql();
            base.Check(@"SELECT  [st].[ID] AS [stid] , [sc].[id] AS [scid]  FROM [STudent] st Left JOIN School sc ON ( [st].[SchoolId] =[sc].[id])   WHERE ( [st].[ID] = @Id0 )  AND ( [sc].[id] = @Id1 )  AND ( [sc].[id] = [st].[ID] )GROUP BY [st].[ID],[sc].[id]ORDER BY [st].[ID] ASC ",
                null, t2.Key, null, " Mapping t2 error");
            var x2 = GetInstance();
        }

        public SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new AttributeConfig() { ConnectionString = Config.ConnectionString, DbType = DbType.SqlServer, IsAutoCloseConnection = true, EntityNamespace= "OrmTest.Models" });
            return db;
        }
    }
}