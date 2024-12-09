using AutoMapper;
using CoreServices.StaticFiles;
using CoreServices.CustomExceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAPI.Business.Models;
using UserAPI.Business.Repository.Interfaces;
using UserAPI.DTO;
using UserAPI.StaticFiles;
using CoreServices.GenericRepository;
using CoreServices.GenericRepository.Interfaces;

namespace UserAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Roles = $"{AuthorizationRoles.ADMIN}")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all the Users
        /// </summary>
        /// <returns>List of Users</returns>
        /// <response code="200">Returns the list of users</response>
        /// <response code="404">If no users are found</response>
        /// <response code="401">If the request is unauthorized</response>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<UserGetDTO>))]
        public async Task<ActionResult<IEnumerable<UserGetDTO>>> GetUsers()
        {
            var users = await _userRepository.GetAll();

            if (!users.Any())
            {
                return NotFound(ErrorMessages.NO_USERS_FOUND);
            }

            var userDto = _mapper.Map<IEnumerable<UserGetDTO>>(users);
            return Ok(userDto);
        }

        /// <summary>
        /// Retrieves a user by their ID
        /// </summary>
        /// <param name="id">The ID of the user</param>
        /// <returns>The user details</returns>
        /// <response code="200">Returns the user details</response>
        /// <response code="404">If the user with specified ID is not found</response>
        /// <response code="401">If the request is unauthorized</response>
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(UserGetDTO))]
        public async Task<ActionResult<UserGetDTO>> GetUser(int id)
        {
            var user = await _userRepository.GetById(id);

            if (user == null)
            {
                return NotFound(ErrorMessages.USER_NOT_FOUND);
            }

            var userDto = _mapper.Map<UserGetDTO>(user);
            return Ok(userDto);
        }

        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="userPostDTO">The user data to create a new user</param>
        /// <returns>Details of the created user</returns>
        /// <response code="200">Returns the created user</response>
        /// <response code="400">If the user data is invalid</response>
        /// <response code="409">If the email is already registered</response>
        /// <response code="401">If the request is unauthorized</response>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(UserGetDTO))]
        public async Task<ActionResult<UserGetDTO>> AddUser(UserPostDTO userPostDTO)
        {
            var user = _mapper.Map<User>(userPostDTO);

            var alreadyRegisteredUser = await _userRepository.GetByEmail(user.Email);

            if (alreadyRegisteredUser != null)
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

        /// <summary>
        /// Updates an existing user by ID
        /// </summary>
        /// <param name="id">The ID of the user to update</param>
        /// <param name="userUpdateDTO">The updated user data</param>
        /// <returns>The updated user details</returns>
        /// <response code="200">Returns the updated user details</response>
        /// <response code="404">If the user with specified ID is not found</response>
        /// <response code="401">If the request is unauthorized</response>
        [HttpPut("{id}")]
        [ProducesResponseType(200, Type = typeof(UserGetDTO))]
        public async Task<ActionResult<UserGetDTO>> UpdateUser(int id, UserUpdateDTO userUpdateDTO)
        {
            var existingUser = await _userRepository.GetById(id);
            if (existingUser == null)
            {
                return NotFound(ErrorMessages.USER_NOT_FOUND);
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
                existingUser.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(userUpdateDTO.Password);
            }

            existingUser.UpdatedAt = DateTime.Now;

            var updatedUser = await _userRepository.Update(existingUser);
            var updatedUserDTO = _mapper.Map<UserGetDTO>(updatedUser);
            return Ok(updatedUserDTO);
        }

        /// <summary>
        /// Deletes an existing user by ID
        /// </summary>
        /// <param name="id">The ID of the user to delete</param>
        /// <returns>Confirmation of deletion</returns>
        /// <response code="200">Returns a confirmation of deletion</response>
        /// <response code="404">If the user with specified ID is not found</response>
        /// <response code="401">If the request is unauthorized</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(200, Type = typeof(UserGetDTO))]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userRepository.GetById(id);
            if (user == null)
            {
                return NotFound(ErrorMessages.USER_NOT_FOUND);
            }

            var result = await _userRepository.Delete(user.Id);
            if (!result)
            {
                throw new Exception(ErrorMessages.INTERNAL_SERVER_ERROR);
            }
            var userDTO = _mapper.Map<UserGetDTO>(user);
            return Ok(userDTO);
        }
    }
}
