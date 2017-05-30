using LibrameCore.Entity;
using LibrameCore.Entity.Descriptors;
using LibrameCore.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LibrameCore.Tests.Entity
{
    public class EntityTests
    {
        [Fact]
        public void UseEntityTest()
        {
            var services = new ServiceCollection();

            // ע��ʵ����
            var connectionString = "Data Source=PC-I74910MQ\\SQLEXPRESS;Initial Catalog=librame_test;Integrated Security=True";
            
            //// Ĭ��ʵ��ģ��ʹ�� DbContextProvider��Ҳ�ɸ���Ϊ�Լ�����Ҫ������ͬʱ�޸���������Դ
            //services.AddEntityFrameworkSqlServer().AddDbContext<DbContextProvider>(options =>
            //{
            //    options.UseSqlServer(connectionString, sql =>
            //    {
            //        sql.UseRowNumberForPaging();
            //        sql.MaxBatchSize(50);
            //    });
            //});

            // ע�� Librame ��Ĭ��ʹ���ڴ�����Դ��
            var builder = services.AddLibrameByMemory(source =>
            {
                //// �޸�Ĭ�ϵ� DbContextProvider
                //source[EntityOptions.EntityProviderTypeNameKey]
                //    = typeof(DbContextProvider).AssemblyQualifiedNameWithoutVcp();

                // ����ʵ�����
                source[EntityOptions.AutomappingAssembliesKey]
                    = TypeUtility.GetAssemblyName<Article>().Name;
            });

            // ��ȡʵ������������֮ǰδע�� AddEntityFrameworkSqlServer���˴�ʹ���ڲ�����ע�ᣬ��������ַ�������Ϊ�գ�
            var adapter = builder.GetEntityAdapter(connectionString);
            
            // ��ʼ������
            var article = new Article
            {
                Title = "Test Title",
                Descr = "Test Descr"
            };
            
            var repository = adapter.GetRepository<Article>();

            // ���ⲻ���ظ�
            Article dbArticle;
            if (!repository.Exists(p => p.Title == article.Title, out dbArticle))
            {
                dbArticle = repository.Create(article);
            }
            else
            {
                article = dbArticle;
            }

            // �Ա�
            Assert.Equal(article.CreateTime, dbArticle.CreateTime);
        }
    }

    
    public class Article : AbstractCreateDataIdDescriptor<int>
    {
        public string Title { get; set; }
        
        public string Descr { get; set; }
    }

}
