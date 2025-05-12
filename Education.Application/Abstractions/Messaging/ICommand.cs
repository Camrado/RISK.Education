﻿using MediatR;

namespace Education.Application.Abstractions.Messaging;

public interface ICommand : IRequest, IBaseCommand
{
}

public interface ICommand<TResponse> : IRequest<TResponse>, IBaseCommand
{
}

public interface IBaseCommand
{
}
