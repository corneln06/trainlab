namespace TrainLab.IntegrationTests.Infrastructure;

[CollectionDefinition(Name)]
public class ApiCollection : ICollectionFixture<TrainLabApiFactory>
{
    public const string Name = "Api";
}