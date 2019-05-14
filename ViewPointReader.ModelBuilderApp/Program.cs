using ViewPointReader.ModelBuilder;
using System;
using System.Threading.Tasks;


namespace ViewPointReader.ModelBuilderApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var modelBuilder = new ModelBuilder.ModelBuilder();

            await modelBuilder.BuildModel();
        }
    }
}
