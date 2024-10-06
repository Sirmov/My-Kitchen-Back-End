namespace MyKitchen.Common.Constants
{
    public class ExceptionMessages
    {
        /// <summary>
        /// A error message indicating that no document was found.
        /// </summary>
        public const string DocumentNotFound = "No document was found";

        /// <summary>
        /// A error message indicating that the provided id does not match the id of the document.
        /// 0 Parameter - The provided id.
        /// 1 Parameter - The id of the document.
        /// </summary>
        public const string DocumentIdsDontMatch = "The provided id does not match the id of the document. The provided id - {0}, the id of the document - {1}";

        /// <summary>
        /// A error message indicating that a variable cannot be null.
        /// 0 Parameter - The name of the variable.
        /// </summary>
        public const string VariableIsNull = "Variable \"{0}\" cannot be null.";

        /// <summary>
        /// A error message indicating that a variable cannot be true.
        /// </summary>
        public const string VariableIsTrue = "Variable \"{0}\" cannot be true.";

        /// <summary>
        /// A error message indicating that a variable cannot be false.
        /// </summary>
        public const string VariableIsFalse = "Variable \"{0}\" cannot be false.";

        /// <summary>
        /// A error message indicating that a variable cannot be null or white space.
        /// 0 Parameter - The name of the variable.
        /// </summary>
        public const string VariableIsNullOrWhiteSpace = "Variable \"{0}\" cannot be null or white space.";

        /// <summary>
        /// A error message indicating that a variable cannot be null or empty.
        /// 0 Parameter - The name of the variable.
        /// </summary>
        public const string VariableIsNullOrEmpty = "Variable \"{0}\" cannot be null or empty.";

        /// <summary>
        /// A error message indicating that a string variable does not match a regex pattern.
        /// 0 Parameter - The name of the variable.
        /// 1 Parameter - The string value of the variable.
        /// 2 Parameter - The regex pattern.
        /// </summary>
        public const string VariableRegexDoesNotMatch = "Variable {0} does not match the regex pattern.\nString - {1}\nRegex - {2}";

        /// <summary>
        /// A error message indicating that a argument cannot be null.
        /// 0 Parameter - The name of the argument.
        /// </summary>
        public const string ArgumentIsNull = "Argument \"{0}\" cannot be null.";

        /// <summary>
        /// A error message indicating that a argument cannot be true.
        /// </summary>
        public const string ArgumentIsTrue = "Argument \"{0}\" cannot be true.";

        /// <summary>
        /// A error message indicating that a argument cannot be false.
        /// </summary>
        public const string ArgumentIsFalse = "Argument \"{0}\" cannot be false.";

        /// <summary>
        /// A error message indicating that a argument cannot be null or white space.
        /// 0 Parameter - The name of the variable.
        /// </summary>
        public const string ArgumentIsNullOrWhiteSpace = "Argument \"{0}\" cannot be null or white space.";

        /// <summary>
        /// A error message indicating that a argument cannot be null or empty.
        /// 0 Parameter - The name of the variable.
        /// </summary>
        public const string ArgumentIsNullOrEmpty = "Argument \"{0}\" cannot be null or empty.";

        /// <summary>
        /// A error message indicating that a string argument does not match a regex pattern.
        /// 0 Parameter - The name of the argument.
        /// 1 Parameter - The string value of the argument.
        /// 2 Parameter - The regex pattern.
        /// </summary>
        public const string ArgumentRegexDoesNotMatch = "Variable {0} does not match the regex pattern.\nString - {1}\nRegex - {2}";

        /// <summary>
        /// A error message indicating that a given subject's model state is not valid.
        /// 0 Parameter - The name of the subject.
        /// </summary>
        public const string InvalidModelState = "{0} has invalid model state.";

        /// <summary>
        /// A error message indicating that no entity with specified property can be found.
        /// 0 Parameter - The name of the entity.
        /// 1 Parameter - The name of the property.
        /// </summary>
        public const string NoEntityWithPropertyFound = "No {0} with this {1} was found.";

        /// <summary>
        /// A error message indicating that the login was not successful.
        /// </summary>
        public const string IncorrectEmailOrPassword = "Incorrect email or password.";

        /// <summary>
        /// A error message indicating that the operation was not successful.
        /// 0 Parameter - The name of the operation.
        /// </summary>
        public const string OperationNotSuccessful = "{0} was not successful.";

        /// <summary>
        /// A error message indicating that the authentication process was not successful.
        /// Either the password or the specified identificator is not correct.
        /// 0 Parameter - The name of the specified identificator.
        /// </summary>
        public const string AuthenticationNotSuccessful = "Authentication was not successful. Password or {0} is not correct.";

        /// <summary>
        /// A error message indicating that the request is not authorized.
        /// </summary>
        public const string Unauthorized = "Access is denied due to invalid credentials.";

        /// <summary>
        /// A error message indicating that something is already in some state.
        /// </summary>
        public const string ObjectIsAlready = "{0} is already {1}.";

        /// <summary>
        /// A error message indicating a variable is set to a null reference.
        /// 0 Parameter - The name of the variable.
        /// </summary>
        public const string NullReference = "{0} is set to a null reference.";

        /// <summary>
        /// A error message indicating that the entity cannot be found.
        /// 0 Parameter - The name of the entity.
        /// </summary>
        public const string EntityNotFound = "The {0} cannot be found.";

        /// <summary>
        /// A error message indicating that the source or destination object is null.
        /// </summary>
        public const string SourceOrDestinationNull = "Source or/and Destination Objects are null";

        /// <summary>
        /// A error message indicating that a controller cannot be assumed and should be specified.
        /// </summary>
        public const string ControlledCanNotBeAssuemd = "Controller cannot be assumed, it has to be specified.";

        /// <summary>
        /// A error message indicating that the deserialization was not successful.
        /// </summary>
        public const string DeserializationFailed = "Deserialization was not successful.";

        /// <summary>
        /// A error message indicating that an unexpected internal server error has occurred.
        /// </summary>
        public const string InternalServerError = "An unexpected error has occurred.";

        /// <summary>
        /// A error message indicating that a user is already in role.
        /// </summary>
        public const string UserAlreadyInRole = "The user cannot be added to a role that he is in.";

        /// <summary>
        /// A error message indicating that a user is not in a role.
        /// </summary>
        public const string UserNotInRole = "The user cannot be removed from a role he is not in.";

        /// <summary>
        /// A error message indicating that a property cannot be found in a object.
        /// 0 Parameter - The name of the property.
        /// 1 Parameter - The type of the object.
        /// </summary>
        public const string PropertyNotFound = "Property '{0}' not found in object of type '{1}'.";

    }
}
