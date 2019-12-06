using CRUD_Operations.Models;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRUD_Operations
{
    public class ElasticContext
    {
        private static readonly ConnectionSettings connSettings =
            new ConnectionSettings(new Uri("http://localhost:9200/"))
                .DefaultIndex("doctors5");
        //Optionally override the default index for specific types
        //.MapDefaultTypeIndices(m => m
        //.Add(typeof(Doctors), "doctors2"));
        private static readonly ElasticClient elasticClient = new ElasticClient(connSettings);

        public void InsertDoctor(Doctors doctors)
        {
            //elasticClient.DeleteIndex("log_history");

            if (!elasticClient.Indices.Exists("doctors5").Exists)
            {
                var indexSettings = new IndexSettings();
                indexSettings.NumberOfReplicas = 1;
                indexSettings.NumberOfShards = 3;


                var createIndexDescriptor = new CreateIndexDescriptor("doctors5")
               .Mappings(ms => ms
                               .Map<Doctors>(m => m.AutoMap())
                        )
                .InitializeUsing(new IndexState() { Settings = indexSettings })
                .Aliases(a => a.Alias("error_log"));

                var response = elasticClient.Indices.Create(createIndexDescriptor);
            }
            //Insert Data           

            elasticClient.Index<Doctors>(doctors, idx => idx.Index("doctors5"));
        }


        public async Task<List<Doctors>> GetAllDoctorsTask()
        {
            var response = await elasticClient.SearchAsync<Doctors>(p => p
                 //.Source(f=>f.Includes(p2=>p2.Field(f2=>f2.message)))                  
                 .Query(q => q
                 .MatchAll()

              //PostFilter(f => f.DateRange(r => r.Field(f2 => f2.Id).GreaterThanOrEquals("0"))
              )
            );
            var result = new List<Doctors>();
            foreach (var document in response.Documents)
            {
                result.Add(document);
            }
            return result.ToList();
        }

        public async Task<List<Doctors>> Get(string id)
        {
            var response = elasticClient.Search<Doctors>(s => s.Query(q => q.
                QueryString(x => x.Query(id))));

            var result = new List<Doctors>();
            foreach (var document in response.Documents)
            {
                result.Add(document);
            }
            return result;
        }

        public void InsertUpdate(Doctors doctors)
        {
            elasticClient.Update<Doctors>(doctors, u => u.Doc(doctors));
        }

        public void Delete(string id)
        {
         //   elasticClient.Delete<Doctors>(id);
            //elasticClient.Delete(new DeleteRequest("doctors5", "doctors", "1"));
            elasticClient.DeleteAsync(new DocumentPath<Doctors>(id));
        }
    }
}
