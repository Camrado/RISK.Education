﻿using System.Net;
using System.Net.Http.Json;
using Education.Application.Languages.CreateLanguage;
using Education.Exceptions.Exceptions;
using Education.Infrastructure;
using Education.Integration.Tests.Common;
using Education.Integration.Tests.Common.Response;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Education.Integration.Tests.Languages;

public class CreateLanguageCommandTests : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient _client;
    private readonly ApplicationDbContext _dbContext;

    public CreateLanguageCommandTests(IntegrationTestFixture fixture)
    {
        _client = fixture.Client;
        _dbContext = fixture.DbContext;
    }

    [Theory]
    [InlineData("uk")]
    public async Task Should_Create_Language_When_ValidDataProvided(string languageCode)
    {
        var request = new CreateLanguageCommand(languageCode);
        var response = await _client.PostAsJsonAsync("/api/languages", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadFromJsonAsync<CreateLanguageCommandResponse>();
        content.Should().NotBeNull();

        var createdLanguage = await _dbContext.Languages.FirstOrDefaultAsync(l => l.Code == languageCode);
        createdLanguage.Should().NotBeNull();
    }

    [Theory]
    [InlineData(LanguageConstants.EmptyLanguageCode)]
    public async Task Should_Return_BadRequest_When_LanguageCodeIsEmpty(string languageCode)
    {
        var request = new CreateLanguageCommand(languageCode);
        var response = await _client.PostAsJsonAsync("/api/languages", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();
        content.Should().NotBeNull();
        content.Type.Should().Be(nameof(ValidationException));
        content.Detail.Should().Be("One or more validation errors occurred.");
        content.Status.Should().Be((int)HttpStatusCode.BadRequest);
        content.Errors.Should().ContainKey("Code");
        content.Errors["Code"].Should().Contain("Code is required.");
    }

    [Theory]
    [InlineData("aze")]
    [InlineData("a")]
    public async Task Should_Return_BadRequest_When_LanguageCodeIsNotTwoCharactersLong(string languageCode)
    {
        var request = new CreateLanguageCommand(languageCode);
        var response = await _client.PostAsJsonAsync("/api/languages", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();
        content.Should().NotBeNull();
        content.Type.Should().Be(nameof(ValidationException));
        content.Detail.Should().Be("One or more validation errors occurred.");
        content.Status.Should().Be((int)HttpStatusCode.BadRequest);
        content.Errors.Should().ContainKey("Code");
        content.Errors["Code"].Should().Contain("Code must be exactly 2 characters long.");
    }

    [Theory]
    [InlineData("ad")]
    [InlineData("fe")]
    public async Task Should_Return_BadRequest_When_LanguageCodeIsNotValid(string languageCode)
    {
        var request = new CreateLanguageCommand(languageCode);
        var response = await _client.PostAsJsonAsync("/api/languages", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();
        content.Should().NotBeNull();
        content.Type.Should().Be(nameof(ValidationException));
        content.Detail.Should().Be("One or more validation errors occurred.");
        content.Status.Should().Be((int)HttpStatusCode.BadRequest);
        content.Errors.Should().ContainKey("Code");
        content.Errors["Code"].Should().Contain("Code must be a valid ISO 639-1 language code.");
    }

    [Theory]
    [InlineData(LanguageConstants.SampleLanguageCode)]
    public async Task Should_Return_Conflict_When_LanguageCodeAlreadyExists(string languageCode)
    {
        var request = new CreateLanguageCommand(languageCode);
        var response = await _client.PostAsJsonAsync("/api/languages", request);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var content = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();
        content.Should().NotBeNull();
        content.Type.Should().Be(nameof(ConflictException));
        content.Detail.Should().Be($"Language with code '{languageCode}' already exists.");
        content.Status.Should().Be((int)HttpStatusCode.Conflict);
    }
}
