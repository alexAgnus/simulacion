# 👾 Arquitectura y convenciones

Para guardar el orden en el proyecto, se propone la siguiente arquitectura:

```bash
├───.vscode
├───Assets
│   ├───CiDy
│   ├───ForestTreePack
│   ├───GleyPlugins
│   ├───GroundTexturesPack
│   ├───TreePack
│   ├─── ... Others packages 
│   └───_Project -> Own creations
│       ├───Images
│       ├───Scenes
│       │   └───Examples -> Only use to examples and test
│       │   └─── ... Add folders to organize scenes, level, menus, etc.
│       └───World
│           ├───Terrain
│           └───TerrainLayers
├───Docs
├───Packages
└───ProjectSettings
```

Dentro de la carpeta `Assets/_Project` es donde se crearán todos los assets que sean propios del proyecto y que sea nuestra responsabilidad mantener.

Toda creación que pertenezca a terceros, debe ir en su carpeta externa a `_Project`.

## 💻 Código y programación

Apegarse a las convensiones y naming usado en C#.Todo el código debe escribirse en inglés.

Agregar documentación dentro de la carpeta `docs`.

## 🔠 Git y Github

Usar Commit Convention al momento de enviar commits a las ramas. Adjunto documentación. [Ver Conventional commits](https://www.conventionalcommits.org/en/v1.0.0/).
