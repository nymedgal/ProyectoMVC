//MENSAJES
function mostrarMensajeError(mensaje) {
    if (typeof Swal !== 'undefined') {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: mensaje
        });
    } else {
        console.error('SweetAlert2 no esta definido');
    }
}
function mostrarMensajeExito(mensaje) {
    if (typeof Swal !== 'undefined') {
        Swal.fire({
            icon: 'success',
            title: 'Exito',
            text: mensaje
        });
    } else {
        console.error('SweetAlert2 no esta definido');
    }
}
function confirmarActualizacion() {
    Swal.fire({
        title: 'Estas seguro?',
        text: "Deseas guardar los cambios?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Sí, guardar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            document.getElementById('editarForm').submit();
        }
    });
}

//ELIMINAR PARAMETRO
function confirmarAccion(mensaje, textoConfirmacion, accion, id, url, idName) {
    Swal.fire({
        title: '¿Estás seguro?',
        text: mensaje,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: textoConfirmacion,
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            ejecutarAccion(id, url, accion, idName);
        }
    });
}
function ejecutarAccion(id, url, accion, idName) {
    fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: new URLSearchParams({
            [idName]: id 
        })
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Error en la respuesta del servidor');
            }
            return response.text();
        })
        .then(data => {
            Swal.fire(
                'Completado!',
                `La acción de ${accion} se ha completado exitosamente.`,
                'success'
            ).then(() => {
                location.reload();
            });
        })
        .catch(error => {
            console.error(`Error al ejecutar la acción de ${accion}:`, error);
            Swal.fire(
                'Error!',
                `No se pudo completar la acción de ${accion}.`,
                'error'
            );
        });
}

//ELIMINAR CON 2 PARAMETROS 
function confirmarAccion2(mensaje, textoConfirmacion, accion, id, id2, url, idName, idName2) {
    Swal.fire({
        title: '¿Estás seguro?',
        text: mensaje,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: textoConfirmacion,
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            ejecutarAccion2(id, id2, url, accion, idName, idName2);
        }
    });
}
function ejecutarAccion2(id, id2, url, accion, idName, idName2) {
    fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: new URLSearchParams({
            [idName]: id,
            [idName2]: id2
        })
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Error en la respuesta del servidor');
            }
            return response.text();
        })
        .then(data => {
            Swal.fire(
                'Completado!',
                `La acción de ${accion} se ha completado exitosamente.`,
                'success'
            ).then(() => {
                location.reload();
            });
        })
        .catch(error => {
            console.error(`Error al ejecutar la acción de ${accion}:`, error);
            Swal.fire(
                'Error!',
                `No se pudo completar la acción de ${accion}.`,
                'error'
            );
        });
}

//ELIMINAR MASIVAMENTE
function eliminarSeleccionados() {
    var empleadosSeleccionados = $(".seleccionarEmpleado:checked").map(function () {
        return $(this).val();
    }).get();

    if (empleadosSeleccionados.length === 0) {
        Swal.fire('Advertencia', 'Debe seleccionar al menos un empleado para eliminar.', 'warning');
    } else {
        // Usar la variable eliminarMasivamenteUrl generada en la vista
        confirmarAccionMasiva('No podrás revertir esta acción', 'Sí, eliminar', empleadosSeleccionados, eliminarMasivamenteUrl);
    }
}
function confirmarAccionMasiva(mensaje, textoConfirmacion, ids, url) {
    Swal.fire({
        title: '¿Estás seguro?',
        text: mensaje,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: textoConfirmacion,
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            ejecutarAccionMasiva(ids, url);
        }
    });
}
function ejecutarAccionMasiva(ids, url) {
    console.log('IDs a eliminar:', ids);

    fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: new URLSearchParams({
            'ids': ids.join(',')  
        })
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Error en la respuesta del servidor');
            }
            return response.text();
        })
        .then(data => {
            Swal.fire(
                'Completado!',
                'Los empleados seleccionados han sido eliminados exitosamente.',
                'success'
            ).then(() => {
                location.reload();
            });
        })
        .catch(error => {
            console.error('Error al eliminar empleados:', error);
            Swal.fire(
                'Error!',
                'No se pudo completar la acción de eliminación.',
                'error'
            );
        });
}

