using Microsoft.AspNetCore.Mvc;
using Shared.DTO;

namespace Shared.ResponseHandler
{
    public static class ResponseHandler<T>
    {
        /// <summary>
        /// Generates an HTTP response based on the provided generic response object.
        /// </summary>
        /// <typeparam name="T">The type of data in the response.</typeparam>
        /// <param name="response">The response object containing the data and status.</param>
        /// <returns>An IActionResult representing the response.</returns>
        public static IActionResult Response(Response<T> response)
        {
            return new ObjectResult(response)
            {
                StatusCode = response.Status
            };
        }
    }
}
