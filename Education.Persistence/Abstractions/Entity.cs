﻿namespace Education.Persistence.Abstractions;

public abstract class Entity {
	public DateTime CreatedAt { get; } = DateTime.UtcNow;
	public DateTime? UpdatedAt { get; private set; } = null;
	public DateTime? DeletedAt { get; private set; } = null;
	
	public void MarkAsUpdated() {
		UpdatedAt = DateTime.UtcNow;
	}
	
	public void MarkAsDeleted() {
		DeletedAt = DateTime.UtcNow;
	}
	
	public bool IsDeleted() {
		return DeletedAt is not null;
	}
	
	public bool IsUpdated() {
		return UpdatedAt is not null;
	}
}