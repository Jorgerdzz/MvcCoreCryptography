using Microsoft.EntityFrameworkCore;
using MvcCoreCryptography.Data;
using MvcCoreCryptography.Helpers;
using MvcCoreCryptography.Models;

namespace MvcCoreCryptography.Repositories
{
    public class RepositoryUsuarios
    {
        private UsuariosContext context;

        public RepositoryUsuarios(UsuariosContext context)
        {
            this.context = context;
        }

        private async Task<int> GetMaxIdUsuariosAsync()
        {
            if (this.context.Usuarios.Count() == 0)
            {
                return 1;
            }
            else
            {
                return await this.context.Usuarios.MaxAsync(u => u.IdUsuario) + 1;
            }
        }

        public async Task RegisterUserAsync(string nombre, string email, string imagen, string password)
        {
            Usuario user = new Usuario();
            user.IdUsuario = await this.GetMaxIdUsuariosAsync();
            user.Nombre = nombre;
            user.Email = email;
            user.Imagen = imagen;
            //CADA USUARIO TENDRA UN SALT DIFERENTE
            user.Salt = HelperTools.GenerateSalt();
            user.Password = HelperCryptography.EncryptPassword(password, user.Salt);
            await this.context.Usuarios.AddAsync(user);
            await this.context.SaveChangesAsync();
        }

        //NECESITAMOS UN METODO PARA VALIDAR AL USUARIO
        //MEDIANTE SU LOGIN Y SU PASSWORD
        //PARA LA VALIDACION DE USUARIOS SIEMPRE DEBEMOS HACERLO
        //MEDIANTE CAMPOS UNIQUE: (email, username, nif...)
        public async Task<Usuario> LogInUserAsync(string email, string password)
        {
            var consulta = from datos in this.context.Usuarios
                           where datos.Email == email
                           select datos;
            Usuario user = await consulta.FirstOrDefaultAsync();
            if(user == null)
            {
                return null;
            }
            else
            {
                //NECESITAMOS EL SALT DEL USUARIO
                string salt = user.Salt;
                //CIFRAMOS EL PASSWORD CON SU SALT A NIVEL DE BYTE[]
                byte[] temp = HelperCryptography.EncryptPassword(password, salt);
                //RECUPERAMOS LOS BYTES[] DEL PASSWORD DE LA BBDD
                byte[] passBytes = user.Password;
                bool reponse = HelperTools.CompareArrays(temp, passBytes);
                if (reponse)
                {
                    return user;
                }
                else
                {
                    return null;
                }
            }
        }

    }
}
