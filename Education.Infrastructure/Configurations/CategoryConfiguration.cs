﻿using Education.Persistence.Categories;
using Education.Persistence.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Configurations;

internal class CategoryConfiguration : SoftDeleteEntityConfiguration<Category> {
    protected override void ConfigureEntity(EntityTypeBuilder<Category> builder) {
        builder.ToTable("categories");
        
        builder.HasKey(c => c.CategoryId);

        builder.Property(c => c.Title)
            .IsRequired();
        
        builder.Property(c => c.Description)
            .IsRequired();

        builder.HasMany(c => c.Courses)
            .WithOne(c => c.Category)
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}