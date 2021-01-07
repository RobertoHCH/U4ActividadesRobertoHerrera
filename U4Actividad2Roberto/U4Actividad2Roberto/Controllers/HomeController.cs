using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using U4Actividad2Roberto.Helpers;
using U4Actividad2Roberto.Models;
using U4Actividad2Roberto.Models.ViewModels;
using U4Actividad2Roberto.Repositories;

namespace U4Actividad2Roberto.Controllers
{
    public class HomeController: Controller
    { 
     [Authorize(Roles = "Director, Docente")]
            public IActionResult Index(int nocontrol)
    {
        return View();
    }
    [AllowAnonymous]
    public IActionResult ISDirector()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> ISDirector(Director dire)
    {
            rolesrobertoContext context = new rolesrobertoContext();
        UsuarioRepository<Director> directorRepository = new UsuarioRepository<Director>(context);
        var director = context.Director.FirstOrDefault(x => x.Clave == dire.Clave);
        try
        {
            if (director != null && director.Contrasena == HashingHelper.GetHash(dire.Contrasena))
            {
                List<Claim> info = new List<Claim>();
                info.Add(new Claim(ClaimTypes.Name, "Usuario" + director.Nombre));
                info.Add(new Claim(ClaimTypes.Role, "Director"));
                info.Add(new Claim("Clave", director.Nombre.ToString()));
                info.Add(new Claim("Nombre", director.Nombre));

                var claimsidentity = new ClaimsIdentity(info, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsprincipal = new ClaimsPrincipal(claimsidentity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsprincipal,
                    new AuthenticationProperties { IsPersistent = true });
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "La clave o la contraseña del director son incorrectas.");
                return View(dire);
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(dire);
        }
    }
    [Authorize(Roles = "Director")]
    public IActionResult RegistrarDocente()
    {
        return View();
    }

    [Authorize(Roles = "Director")]
    [HttpPost]
    public IActionResult RegistrarDocente(Docente dnte)
    {
        rolesrobertoContext context = new rolesrobertoContext();
        DocenteRepository repository = new DocenteRepository(context);

        try
        {
            var maestro = repository.GetDocenteByClave(dnte.Clave);
            if (maestro == null)
            {
                dnte.Activo = 1;
                dnte.Contrasena = HashingHelper.GetHash(dnte.Contrasena);
                repository.Insert(dnte);
                return RedirectToAction("VDocente");
            }
            else
            {
                ModelState.AddModelError("", "Clave no está disponible.");
                return View(dnte);
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(dnte);
        }
    }



    [AllowAnonymous]
    public IActionResult ISDocente()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> ISDocente(Docente dnte)
    {
        rolesrobertoContext context = new rolesrobertoContext();
        DocenteRepository repository = new DocenteRepository(context);
        var docent = repository.GetDocenteByClave(dnte.Clave);
        try
        {
            if (docent != null && docent.Contrasena == HashingHelper.GetHash(dnte.Contrasena))
            {
                if (docent.Activo == 1)
                {
                    List<Claim> info = new List<Claim>();
                    info.Add(new Claim(ClaimTypes.Name, "Usuario" + docent.Nombre));
                    info.Add(new Claim(ClaimTypes.Role, "Docente"));
                    info.Add(new Claim("Clave", docent.Clave.ToString()));
                    info.Add(new Claim("Nombre", docent.Nombre));
                    info.Add(new Claim("Id", docent.Id.ToString()));

                    var claimsidentity = new ClaimsIdentity(info, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsprincipal = new ClaimsPrincipal(claimsidentity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsprincipal,
                        new AuthenticationProperties { IsPersistent = true });
                    return RedirectToAction("Index", docent.Clave);
                }
                else
                {
                    ModelState.AddModelError("", "Atencion, Cuenta desactivada.");
                    return View(dnte);
                }
            }
            else
            {
                ModelState.AddModelError("", "La clave o la contraseña del docente son incorrectas.");
                return View(dnte);
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(dnte);
        }
    }

    [AllowAnonymous]
    public async Task<IActionResult> CerrarSesion()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Index");
    }

    [Authorize(Roles = "Director")]
    public IActionResult VDocente()
    {
        rolesrobertoContext context = new rolesrobertoContext();
        DocenteRepository dnteRepos = new DocenteRepository(context);
        var docent = dnteRepos.GetAll();
        return View(docent);
    }


    [Authorize(Roles = "Director")]
    [HttpPost]
    public IActionResult ActivarDocente(Docente dnte)
    {
        rolesrobertoContext context = new rolesrobertoContext();
        DocenteRepository docenteRepos = new DocenteRepository(context);
        var docent = docenteRepos.GetById(dnte.Id);
        if (docent != null && docent.Activo == 0)
        {
            docent.Activo = 1;
            docenteRepos.Edit(docent);
        }
        else
        {
            docent.Activo = 0;
            docenteRepos.Edit(docent);
        }
        return RedirectToAction("VDocente");
    }

    [Authorize(Roles = "Director")]
    public IActionResult EditarDocente(int id)
    {
        rolesrobertoContext context = new rolesrobertoContext();
        DocenteRepository docenteRepos = new DocenteRepository(context);
        var docent = docenteRepos.GetById(id);
        if (docent != null)
        {
            return View(docent);
        }
        return RedirectToAction("VDocente");
    }

    [Authorize(Roles = "Director")]
    [HttpPost]
    public IActionResult EditarDocente(Docente dnte)
    {
        rolesrobertoContext context = new rolesrobertoContext();
        DocenteRepository deocenteRepos = new DocenteRepository(context);
        var docent = deocenteRepos.GetById(dnte.Id);
        try
        {
            if (docent != null)
            {
                docent.Nombre = dnte.Nombre;
                deocenteRepos.Edit(docent);
            }
            return RedirectToAction("VDocente");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(docent);
        }
    }

    [Authorize(Roles = "Director")]
    public IActionResult ModificarContrasena(int id)
    {
        rolesrobertoContext context = new rolesrobertoContext();
        DocenteRepository repository = new DocenteRepository(context);
        var maestro = repository.GetById(id);
        if (maestro == null)
        {
            return RedirectToAction("VDocente");
        }
        return View(maestro);
    }

    [Authorize(Roles = "Director")]
    [HttpPost]
    public IActionResult ModificarContrasena(Docente dnte, string nuevaContra, string confirmarContra)
    {
        rolesrobertoContext context = new rolesrobertoContext();
        DocenteRepository docenteRepos = new DocenteRepository(context);
        var docent = docenteRepos.GetById(dnte.Id);
        try
        {

            if (docent != null)
            {
                if (nuevaContra != confirmarContra)
                {
                    ModelState.AddModelError("", "Las nuevas contraseñas no son las mismas.");
                    return View(docent);
                }
                else if (docent.Contrasena == HashingHelper.GetHash(nuevaContra))
                {
                    ModelState.AddModelError("", "Ingreso una contraseña anterior, intentelo con una nueva.");
                    return View(docent);
                }
                else
                {
                    docent.Contrasena = HashingHelper.GetHash(nuevaContra);
                    docenteRepos.Edit(docent);
                    return RedirectToAction("VDocente");
                }
            }
            else
            {
                ModelState.AddModelError("", "El docente al que intentó modificar no existe.");
                return View(docent);
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(docent);
        }
    }

    [Authorize(Roles = "Director, Docente")]
    public IActionResult VAlumno(int id)
    {
        rolesrobertoContext context = new rolesrobertoContext();
        DocenteRepository docenteRepos = new DocenteRepository(context);
        var docent = docenteRepos.GetAlumnoByDocente(id);
        if (docent != null)
        {
            if (User.IsInRole("Docente"))
            {
                if (User.Claims.FirstOrDefault(x => x.Type == "Id").Value == docent.Id.ToString())
                {
                    return View(docent);
                }
                else
                {
                    return RedirectToAction("Error");
                }
            }
            else
            {
                return View(docent);
            }
        }
        else
        {
            return RedirectToAction("VAlumno");
        }
    }

    [Authorize(Roles = "Director, Docente")]
    public IActionResult AgregarAlumno(int id)
    {
        rolesrobertoContext context = new rolesrobertoContext();
        DocenteRepository docenteRepos = new DocenteRepository(context);
        AgregarAlumVM vm = new AgregarAlumVM();
        vm.Docente = docenteRepos.GetById(id);
        if (vm.Docente != null)
        {
            if (User.IsInRole("Docente"))
            {
                if (User.Claims.FirstOrDefault(x => x.Type == "Id").Value == vm.Docente.Id.ToString())
                {
                    return View(vm);
                }
                else
                {
                    return RedirectToAction("Error");
                }
            }
            else
            {
                return View(vm);
            }
        }
        return View(vm);
    }

    [Authorize(Roles = "Director, Docente")]
    [HttpPost]
    public IActionResult AgregarAlumno(AgregarAlumVM vm)
    {
        rolesrobertoContext context = new rolesrobertoContext();
        DocenteRepository docenteRepos = new DocenteRepository(context);
        AlumnosRepository alumnosRepos = new AlumnosRepository(context);
        try
        {
            if (context.Alumno.Any(x => x.NumeroDeControl == vm.Alumno.NumeroDeControl))
            {
                ModelState.AddModelError("", "Este número de control ya se encuentra registrado.");
                return View(vm);
            }
            else
            {
                var maestro = docenteRepos.GetDocenteByClave(vm.Docente.Clave).Id;
                vm.Alumno.IdDocente = maestro;
                alumnosRepos.Insert(vm.Alumno);
                return RedirectToAction("VAlumno", new { id = maestro });
            }

        }
        catch (Exception ex)
        {
            vm.Docente = docenteRepos.GetById(vm.Docente.Id);
            vm.lstDocentes = docenteRepos.GetAll();
            ModelState.AddModelError("", ex.Message);
            return View(vm);
        }
    }

    [Authorize(Roles = "Director, Docente")]
    public IActionResult EditarAlumno(int id)
    {
        rolesrobertoContext context = new rolesrobertoContext();
        DocenteRepository docenteRepos = new DocenteRepository(context);
        AlumnosRepository alumnosRepos = new AlumnosRepository(context);
        AgregarAlumVM vm = new AgregarAlumVM();
        vm.Alumno = alumnosRepos.GetById(id);
        vm.lstDocentes = docenteRepos.GetAll();
        if (vm.Alumno != null)
        {
            vm.Docente = docenteRepos.GetById(vm.Alumno.Id);
            if (User.IsInRole("Docente"))
            {
                vm.Docente = docenteRepos.GetById(vm.Alumno.IdDocente);
                if (User.Claims.FirstOrDefault(x => x.Type == "Clave").Value == vm.Docente.Clave.ToString())
                {

                    return View(vm);
                }
            }
            return View(vm);

        }
        else return RedirectToAction("Index");
    }

    [Authorize(Roles = "Director, Docente")]
    [HttpPost]
    public IActionResult EditarAlumno(AgregarAlumVM vm)
    {
        rolesrobertoContext context = new rolesrobertoContext();
        DocenteRepository docenteRepos = new DocenteRepository(context);
        AlumnosRepository alumnosRepos = new AlumnosRepository(context);
        try
        {
            var alumno = alumnosRepos.GetById(vm.Alumno.Id);
            if (alumno != null)
            {
                alumno.Nombre = vm.Alumno.Nombre;
                alumnosRepos.Edit(alumno);
                return RedirectToAction("VAlumno", new { id = alumno.IdDocente });
            }
            else
            {
                ModelState.AddModelError("", "El alumno que intentó editar no existe.");
                vm.Docente = docenteRepos.GetById(vm.Alumno.IdDocente);
                vm.lstDocentes = docenteRepos.GetAll();
                return View(vm);
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            vm.Docente = docenteRepos.GetById(vm.Alumno.IdDocente);
            vm.lstDocentes = docenteRepos.GetAll();
            return View(vm);
        }
    }

    [Authorize(Roles = "Director, Docente")]
    [HttpPost]
    public IActionResult EliminarAlumno(Alumno a)
    {
        rolesrobertoContext context = new rolesrobertoContext();
        AlumnosRepository alumnosRepos = new AlumnosRepository(context);
        var alumno = alumnosRepos.GetById(a.Id);
        if (alumno != null)
        {
            alumnosRepos.Delete(alumno);
        }
        else
        {
            ModelState.AddModelError("", "El alumno que intentó eliminar no existe.");
        }
        return RedirectToAction("VAlumno", new { id = alumno.IdDocente });
    }

    [AllowAnonymous]
    public IActionResult Error()
    {
        return View();
    }
}
}
