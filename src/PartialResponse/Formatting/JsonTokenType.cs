namespace PartialResponse.Net.Http.Formatting
{
    /// <summary>
    /// Specifies the type of a JSON token.
    /// </summary>
    public enum JsonTokenType
    {
        /// <summary>
        /// Represents a JSON value.
        /// </summary>
        Value,

        /// <summary>
        /// Represents a JSON object.
        /// </summary>
        Object,

        /// <summary>
        /// Represents a JSON array.
        /// </summary>
        Array
    }
}
