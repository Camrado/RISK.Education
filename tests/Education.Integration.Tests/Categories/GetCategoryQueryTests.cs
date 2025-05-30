﻿using System.Net;
using System.Net.Http.Json;
using Education.Application.Categories.GetCategory;
using Education.Exceptions.Exceptions;
using Education.Infrastructure;
using Education.Integration.Tests.Common;
using Education.Integration.Tests.Common.Response;
using FluentAssertions;

namespace Education.Integration.Tests.Categories;

public class GetCategoryQueryTests : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient _client;
    private readonly ApplicationDbContext _dbContext;

    public GetCategoryQueryTests(IntegrationTestFixture fixture)
    {
        _client = fixture.Client;
        _dbContext = fixture.DbContext;
    }

    [Theory]
    [InlineData(CategoryConstants.SampleCategoryId, CategoryConstants.SampleCategoryName)]
    public async Task Should_Return_Category_When_CategoryExists(int categoryId, string categoryName)
    {
        var response = await _client.GetAsync($"/api/categories/{categoryId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<GetCategoryQueryResponse>();
        content.Should().NotBeNull();
        content.Id.Should().Be(categoryId);
        content.Name.Should().Be(categoryName);
    }

    [Theory]
    [InlineData(CategoryConstants.NonExistentCategoryId)]
    public async Task Should_Return_NotFound_When_CategoryDoesNotExist(int categoryId)
    {
        var response = await _client.GetAsync($"/api/categories/{categoryId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var content = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();
        content.Should().NotBeNull();
        content.Type.Should().Be(nameof(NotFoundException));
        content.Detail.Should().Be($"Category with ID '{categoryId}' not found.");
        content.Status.Should().Be((int)HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData(CategoryConstants.EmptyCategoryId)]
    public async Task Should_Return_BadRequest_When_CategoryIdIsEmpty(int categoryId)
    {
        var response = await _client.GetAsync($"/api/categories/{categoryId}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();
        content.Should().NotBeNull();
        content.Type.Should().Be(nameof(ValidationException));
        content.Detail.Should().Be("One or more validation errors occurred.");
        content.Status.Should().Be((int)HttpStatusCode.BadRequest);
        content.Errors.Should().ContainKey("CategoryId");
        content.Errors["CategoryId"].Should().Contain("Category ID must not be empty.");
    }
}
