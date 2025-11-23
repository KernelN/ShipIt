# ShipIt
**Ship it** is a mobile game about doing deliveries across space.
Made by Alejandro Nicolas Insausti *(NIn)*.
Third Party Assets credited in *ThirdPartyMentions.md*

## Design Patterns used
***note:** Builder Pattern was schemed, but not used.*

### Factory
Used in:
- MapFactory > SimpleMapFactory
- AstralBodyFactory > PlanetFactory

Used to:
- Modify level generation logic and values. 
 - This allows to either easily change the map generation logic on the future or make different versions of the same level by just tweaking the factory values.
- Unify AstralBody generation logic. 
 - This allows either for different types of the same AstralBody or easier implementation of new Bodies (in e.g. a future HazardousCloud instead of a Planet).

### Composite
Used in:
- AstralBody (Client) & AstralComponent (Leaf)

Used for:
- Adding logic and different behaviours to an object, without the need to making specialized scripts for each body.
 - This way, it's possible to have both a simple Planet (it just rotates), a HazardousPlanet (it rotates and deals damage) and a HazardousCloud (it just deals damage) without the need to make any new scripts.
- Opening the possibility to adding behaviours which contain other behaviours. There was no need for this yet.

### Facade
Used in:
- AstralBody (Facade) & Ship (Client)

Used for:
- Simplifying the interactions between the ship and all of the components of the AstralBody. 
 - AstralBody works as a Facade for Ship, allowing it to simply communicate when both bodies entered and exited collision. AstralBody then communicates all the necessary data to it's components using flags.