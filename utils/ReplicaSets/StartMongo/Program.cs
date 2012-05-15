using MongoDB.Azure.ReplicaSets.ReplicaSetRole;

namespace StartMongo
{
    class Program
    {
        static void Main(string[] args)
        {
            var role = new ReplicaSetRole();
            role.OnStart();
            role.Run();
        }
    }
}
