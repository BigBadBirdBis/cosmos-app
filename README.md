# Cosmos App

## Patterns

### Schema Versionning

[Cosmos Pattern Github](https://github.com/Azure-Samples/cosmos-db-design-patterns/tree/main/schema-versioning)

### Document Versionning

[mongodb blog](https://www.mongodb.com/blog/post/building-with-patterns-the-document-versioning-pattern)

## Good Practises

### Cosmos

[climbtheladder](https://climbtheladder.com/10-cosmos-db-best-practices/)

## Notes

- If the containers are recreated, need to restart the app for CandidateNoteProcessorService.
- Cosmos doesn't allow multi-collections query
- Choice of Partition keys, see : [Microsoft Learn](https://learn.microsoft.com/en-us/azure/cosmos-db/hierarchical-partition-key)
