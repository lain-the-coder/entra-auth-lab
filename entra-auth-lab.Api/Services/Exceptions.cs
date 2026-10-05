namespace entra_auth_lab.Api.Services;

public class NotFoundException(string message) : Exception(message);

public class ValidationException(string message) : Exception(message);
public class ConflictException(string message) : Exception(message);