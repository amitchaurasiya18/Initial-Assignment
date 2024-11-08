using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAPI.Business.Models;
using UserAPI.Business.Repository.Interfaces;
using UserAPI.Business.Services.Interfaces;
using UserAPI.CustomExceptions;
using UserAPI.DTO;
using UserAPI.StaticFiles;

namespace UserAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserGetDTO>>> GetUsers()
        {
            var users = await _userRepository.GetAll();

            if (users == null)
            {
                throw new NoUsersFound(ErrorMessages.NO_USERS_FOUND);
            }

            var userDto = _mapper.Map<IEnumerable<UserGetDTO>>(users);
            return Ok(userDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserGetDTO>> GetUser(int id)
        {
            var user = await _userRepository.GetById(id);

            if (user == null)
            {
                throw new UserNotFound(ErrorMessages.USER_NOT_FOUND);
            }

            var userDto = _mapper.Map<UserGetDTO>(user);
            return Ok(userDto);
        }

        [HttpPost]
        public async Task<ActionResult<UserGetDTO>> AddUser(UserPostDTO userPostDTO)
        {
            var user = _mapper.Map<User>(userPostDTO);

            var alreadyRegisteredUser = await _userRepository.GetByEmail(user.Email);

            if (alreadyRegisteredUser!= null)
            {
                throw new EmailAlreadyRegistered(user.Email + ErrorMessages.EMAIL_ALREADY_REGISTERED);
            }

            user.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(user.Password);
            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;

            var result = await _userRepository.Add(user);
            var userDto = _mapper.Map<UserGetDTO>(result);
            return Ok(userDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserGetDTO>> UpdateUser(int id, UserUpdateDTO userUpdateDTO)
        {
            var existingUser = await _userRepository.GetById(id);
            if (existingUser == null)
            {
                throw new UserNotFound(ErrorMessages.USER_NOT_FOUND);
            }
            if (!string.IsNullOrEmpty(userUpdateDTO.Username))
            {
                existingUser.Username = userUpdateDTO.Username;
            }

            if (!string.IsNullOrEmpty(userUpdateDTO.Email))
            {
                existingUser.Email = userUpdateDTO.Email;
            }

            if (!string.IsNullOrEmpty(userUpdateDTO.Password))
            {
                existingUser.Password = userUpdateDTO.Password;
            }

            existingUser.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(userUpdateDTO.Password);
            existingUser.UpdatedAt = DateTime.Now;

            var updatedUser = await _userRepository.Update(existingUser);
            var updatedUserDTO = _mapper.Map<UserGetDTO>(updatedUser);
            return Ok(updatedUserDTO);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<UserGetDTO>> DeleteUser(int id)
        {
            var user = await _userRepository.GetById(id);
            if (user == null)
            {
                throw new UserNotFound(ErrorMessages.USER_NOT_FOUND);
            }
            var result = await _userRepository.Delete(id);
            return Ok(result);
        }
    }
}