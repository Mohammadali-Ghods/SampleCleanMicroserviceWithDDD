﻿using Amazon.SecurityToken.Model;
using Application.Interface;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetFriendsByUser : IRequestHandler<GetFriendByUserRequest, GetFriendsByUserResponse>
    {
        private readonly IUserFriendsRepository _userFriendsRepository;
        private readonly IUserAPI _userAPI;
        public GetFriendsByUser(IUserFriendsRepository userFriendsRepository,
            IUserAPI userAPI)
        {
            _userFriendsRepository = userFriendsRepository;
            _userAPI = userAPI;
        }
        public async Task<GetFriendsByUserResponse> Handle(GetFriendByUserRequest request, CancellationToken cancellationToken)
        {
            var response = new GetFriendsByUserResponse()
            { Users = new List<Base.ViewModels.BriefUserViewModel>() };

            var result = await _userFriendsRepository.GetById(request.GetUser());
            if (result == null) return response;

            for (int i = 0; i <= result.Friends.Count - 1; i++)
            {
                if (result.Friends[i].Statuses[result.Friends[i].Statuses.Count - 1].Action == Domain.ValueObjects.StatusList.AcceptSent
                    || result.Friends[i].Statuses[result.Friends[i].Statuses.Count - 1].Action == Domain.ValueObjects.StatusList.AcceptReceived)
                {
                    var user = await _userAPI.GetUser(result.Friends[i].UserID);
                    user.Date = result.Friends[i].Statuses[result.Friends[i].Statuses.Count - 1].ActionDate.Value;
                    response.Users.Add(user);
                }
            }

            return response;
        }
    }
}
