﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Tyrion
{
    public sealed class Tyrion : ITyrion
    {
        private readonly IServiceProvider _serviceProvider;

        public Tyrion(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public async Task<IResult<TReponse>> Execute<TRequest, TReponse>(TRequest request) where TRequest : IRequest
        {
            var validator = await ValidateRequestAsync(request);

            if (validator.Failed)
            {
                return Result<TReponse>.Fail(validator.Message);
            }

            var service = _serviceProvider.GetService<IRequestHandler<TRequest, TReponse>>();

            if (service == default)
            {
                throw new ArgumentException($"Class implementing {nameof(IRequestHandler<TRequest, TReponse>)} not found or not implemented!");
            }

            return await service.Execute(request).ConfigureAwait(false);
        }

        public async Task<IResult> Execute<TRequest>(TRequest request) where TRequest : IRequest
        {
            var validator = await ValidateRequestAsync(request);

            if (validator.Failed)
            {
                return Result.Fail(validator.Message);
            }

            var service = _serviceProvider.GetService<IRequestHandler<TRequest>>();

            if (service == default)
            {
                throw new ArgumentException($"Class implementing {nameof(IRequestHandler<TRequest>)} not found or not implemented!");
            }

            await service.Execute(request).ConfigureAwait(false);

            return await Result.SuccessAsync();
        }

        private async Task<IResult> ValidateRequestAsync<TRequest>(TRequest request) where TRequest : IRequest
        {
            var validator = _serviceProvider.GetService<Validator<TRequest>>();

            if (validator == default)
            {
                return await Result.SuccessAsync();
            }

            return await validator.ValidateAsync(request);
        }
    }
}