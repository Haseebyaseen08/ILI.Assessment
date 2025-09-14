using Data.Entities;
using Data.Repositories.ApiUsage;
using Data.Repositories.Customer;
using Data.Repositories.MonthlyUsage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Shared.DTO.ApiUsage;
using Shared.Enums;
using Shared.Settings;

namespace Application.MonthlyUsage
{
    public class MonthlyUsageService : IMonthlyUsageService
    {
        private readonly IMonthlyUsageSummaryRepository _monthlyUsageSummaryRepository;
        private readonly IApiUsageRepository _apiUsageRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<MonthlyUsageService> _logger;
        private readonly RateLimitOptions _rateLimitOptions;

        public MonthlyUsageService(
            IMonthlyUsageSummaryRepository monthlyUsageSummaryRepository,
            IApiUsageRepository apiUsageRepository,
            ICustomerRepository customerRepository,
            ILogger<MonthlyUsageService> logger,
            IOptions<RateLimitOptions> rateLimitOptions)
        {
            _monthlyUsageSummaryRepository = monthlyUsageSummaryRepository;
            _apiUsageRepository = apiUsageRepository;
            _customerRepository = customerRepository;
            _logger = logger;
            _rateLimitOptions = rateLimitOptions.Value;
        }

        public async Task<List<MonthlyUsageSummaryDto>> GetCustomerMonthlySummariesAsync(int customerId)
        {
            var summaries = await _monthlyUsageSummaryRepository.GetByCustomerAsync(customerId);
            return summaries.Select(MapToDto).ToList();
        }

        public async Task<MonthlyUsageSummaryDto?> GetCustomerMonthlySummaryAsync(int customerId, int year, int month)
        {
            var summary = await _monthlyUsageSummaryRepository.GetByCustomerAndPeriodAsync(customerId, year, month);
            return summary != null ? MapToDto(summary) : null;
        }

        public async Task<List<MonthlyUsageSummaryDto>> GetCustomerMonthlySummariesForPeriodAsync(int customerId, DateTime startDate, DateTime endDate)
        {
            var summaries = await _monthlyUsageSummaryRepository.GetByCustomerForPeriodAsync(customerId, startDate, endDate);
            return summaries.Select(MapToDto).ToList();
        }

        public async Task<bool> RegenerateMonthlySummaryAsync(int customerId, int year, int month)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(customerId);
                if (customer == null)
                {
                    _logger.LogWarning("Customer {CustomerId} not found", customerId);
                    return false;
                }

                return await GenerateMonthlySummaryForCustomerAsync(customer, year, month);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error regenerating monthly summary for customer {CustomerId}, year {Year}, month {Month}", customerId, year, month);
                return false;
            }
        }

        public async Task<bool> GenerateMonthlySummaryForAllCustomersAsync(int year, int month)
        {
            try
            {
                var customers = await _customerRepository.GetActiveCustomersAsync();
                var success = true;

                foreach (var customer in customers)
                {
                    try
                    {
                        var result = await GenerateMonthlySummaryForCustomerAsync(customer, year, month);
                        if (!result)
                        {
                            success = false;
                            _logger.LogWarning("Failed to generate monthly summary for customer {CustomerId}", customer.Id);
                        }
                    }
                    catch (Exception ex)
                    {
                        success = false;
                        _logger.LogError(ex, "Error generating monthly summary for customer {CustomerId}", customer.Id);
                    }
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating monthly summaries for all customers, year {Year}, month {Month}", year, month);
                return false;
            }
        }

        private async Task<bool> GenerateMonthlySummaryForCustomerAsync(Customer customer, int year, int month)
        {
            // Get usage data for the month
            var totalApiCalls = await _apiUsageRepository.GetMonthlyUsageCountAsync(customer.Id, year, month);
            var endpointUsage = await _apiUsageRepository.GetMonthlyEndpointUsageAsync(customer.Id, year, month);

            // Get pricing information based on subscription type
            var subscriptionTypeName = customer.SubscriptionType.ToString();
            var pricing = GetPricingForSubscription(subscriptionTypeName);

            var monthlySummary = new MonthlyUsageSummary
            {
                CustomerId = customer.Id,
                Year = year,
                Month = month,
                SubscriptionType = customer.SubscriptionType,
                TotalApiCalls = totalApiCalls,
                PricePerCall = pricing.PricePerCall,
                TotalCost = totalApiCalls * pricing.PricePerCall,
                EndpointUsageJson = JsonConvert.SerializeObject(endpointUsage),
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            return await _monthlyUsageSummaryRepository.CreateOrUpdateSummaryAsync(monthlySummary);
        }

        private PricePerMonth GetPricingForSubscription(string subscriptionType)
        {
            if (_rateLimitOptions.Plans.TryGetValue(subscriptionType, out var rateLimitConfig))
            {
                return rateLimitConfig.PricePerMonth;
            }

            // Default pricing if subscription type not found
            _logger.LogWarning("Pricing not found for subscription type: {SubscriptionType}, using default", subscriptionType);
            return new PricePerMonth
            {
                PricingPerMonth = 0,
                Currency = "USD",
                PricePerCall = 0
            };
        }

        private static MonthlyUsageSummaryDto MapToDto(MonthlyUsageSummary summary)
        {
            var endpointUsage = new Dictionary<string, int>();
            try
            {
                endpointUsage = JsonConvert.DeserializeObject<Dictionary<string, int>>(summary.EndpointUsageJson) ?? new Dictionary<string, int>();
            }
            catch (Exception)
            {
                // If deserialization fails, use empty dictionary
            }

            return new MonthlyUsageSummaryDto
            {
                Id = summary.Id,
                CustomerId = summary.CustomerId,
                CustomerName = summary.Customer?.CustomerName ?? "",
                CompanyName = summary.Customer?.CompanyName ?? "",
                Year = summary.Year,
                Month = summary.Month,
                SubscriptionType = summary.SubscriptionType,
                TotalApiCalls = summary.TotalApiCalls,
                PricePerCall = summary.PricePerCall,
                TotalCost = summary.TotalCost,
                CreatedDate = summary.CreatedDate,
                UpdatedDate = summary.UpdatedDate,
                EndpointUsage = endpointUsage
            };
        }
    }
}