﻿namespace Education.Persistence.Abstractions;

public record Error(string Code, string Name) {
    public static Error None = new(string.Empty, String.Empty);

    public static Error NullValue = new("Error.NullValue", "The value cannot be null.");
}