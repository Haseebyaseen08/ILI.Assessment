using Microsoft.AspNetCore.Http;

namespace Shared.DTO
{
    public record Response<T>
    {
        /// <summary>
        /// Gets or sets the data payload of the response.
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Gets or sets the total count of data elements (if applicable).
        /// </summary>
        public int TotalData { get; set; }

        /// <summary>
        /// Gets or sets the page number of data elements (if applicable).
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Gets or sets the page size of data (if applicable).
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code associated with the response. Defaults to 200 (OK).
        /// </summary>
        public int Status { get; set; } = StatusCodes.Status200OK;

        /// <summary>
        /// Gets or sets an optional message associated with the response.
        /// </summary>
        public string? Message { get; set; }
    }
}
