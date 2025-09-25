## NORWEGIAN ## Oppgave Beskrivelse ##

Det skal lages en Blog løsning som gjør det mulig å opprette flere blogger.

En blogg skal kunne ha et eller flere innlegg (posts) og 
hvert innlegg kan ha en eller flere kommentarer. 

En database benyttes som backend for lagring av alle objekter i løsningen. 

Blog (1:N) → Post (1:N) → Comment
    ↓           ↓           ↓
  OwnerId    OwnerId    OwnerId
    ↓           ↓           ↓
IdentityUser ←───────────────┘

## Oppgavens krav ##
Benytt ASP.NET Core med nødvendige klasser, og Entity Framework, 
samt benytt Repository design mønster.

Følgende funksjonalitet er ønskelig:

Brukere må logge inn for å kunne opprette  en blog, poste nye innlegg og kommentarer, 
innlogget bruker blir eier av objekter som opprettes.

Det skal være mulighet for å få en oversikt over hvilke blogger som eksisterer,
en oversikt over alle innlegg (posts) i en blogg skal kunne vises.

For hvert innlegg bør alle kommentarer kunne vises.

Løsningen bør tilby CRUD (Create, Read, Update og Delete) funksjonalitet 
for innlegg (posts), 
for en blog er det nok med CR (Create og Read), 
dvs. at denne kan opprettes og leses (ikke krav til endring & sletting).

Eier av et objekt (innlegg og kommentar) skal kunne endre og slette dette,  
ved sletting av container objekt skal alle barn slettes. 
Benytt helst IAuthorizationService og IAuthorizationHandler 
for styring av tilgang.

En blogg skal kunne være åpent eller lukket for nye innlegg og kommentarer.

Parameteroverføring skal gå med GET og POST, sesjoner skal ikke benyttes.

## Testing ##
Tilnærmet 100% på egne Controllere (som din BlogController, PostController...)

-------------------------------------------------------------------
------------------------------------------------------------------- 
## ENGLISH ## The task description ENGLISH ##

A testament educational project-excercise. 
A blog solution will be created that makes it possible 
  to create more blogs. 
A blog should be able to have one or more posts (posts) 
  and each post may have one or more comments. 
A local database (preferrably but not necessary SQLite) 
  is used as a backend for storing all objects in the solution. 
Use asp.net Core with necessary classes, and Entity Framework, 
  as well as use the Repository Design pattern.

## The requirements ##
The following functionality is desirable:

1. Users must log in to create a blog, post new posts and comments, 
    the logged-in  user becomes the owner of the objects created.

2. It should be possible to get an overview of which blogs exist, 
    an overview of all posts in a blog should be able to appear.

3. For each post, all comments should be displayed.

4. The solution should offer CRUD (Create, Read, Update and Delete) 
    functionality for Posts. For a blog, CR (Create and Read) are enough, 
    i.e. it can be created and read (no requirements now for change and deletion).

5. The owner of an object (Post and Comment) should be able to change and delete it. 
    When deleting the container object, all children must be deleted. 
    Preferably use the IAuthorizationsService and IAuthorizationsHandler for access.

6. A blog should be open or closed to new posts and comments. 

7. Parameter passing should go with Get and Post, sessions should not be used.

## Testing ##
Approximately 100% on own controllers (like your blog controller, post controller ...). 
I prefer to make tests only for BlogController, PostController.