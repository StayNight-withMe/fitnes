namespace fitnes.Domain.Enums;

public enum Errors
{
    None = 0,
    
    // User & Session
    UserNotFound = 10,
    SessionNotFound = 11,
    SessionExpired = 12,
    Unauthorized = 13,
    
    // Collection
    CollectionNotFound = 20,
    CollectionAlreadyExists = 21,
    EmptyCollection = 22,
    
    // Endpoint
    EndpointNotFound = 30,
    InvalidEndpointUrl = 31,
    DuplicateEndpoint = 32,
    
    // General
    ValidationError = 40,
    InternalError = 50,
    ExternalApiError = 51
}
