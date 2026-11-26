# Ikoner fra game-icons.net

SVG-filer i denne mappe bundtes med frontenden og vises med `<GameIcon name="..." />`.
Hvilket ikon der bruges hvor, står i [`src/utils/icons.ts`](../../utils/icons.ts).

## Tilføj et ikon

1. Find ikonet på <https://game-icons.net> og tryk **Download → SVG**.
   Standardudgaven (hvidt ikon på sort baggrund) er fin: baggrunden fjernes automatisk,
   og ikonet får tekstfarven, så det passer til Bootstrap.
2. Læg filen her i mappen. Filnavnet skal matche `file` i `icons.ts`, fx `pointy-hat.svg`.
3. Skriv ophavspersonen i tabellen nedenfor (står på ikonets side, fx "by Lorc").

Indtil filen ligger her, vises emojien fra `fallback`. I udviklingskonsollen står der, hvilke filer der mangler.

**Nyt sted at bruge et ikon:** tilføj en linje i `iconRegistry` i `icons.ts` og brug navnet:

```tsx
<GameIcon name="hat" className="fs-1 text-primary" />
```

**Farvelagt ikon:** har du selv valgt farver i game-icons.net's editor, så sæt `colors: true`
på ikonet i `icons.ts`, ellers bliver det ensfarvet.

## Licens

Ikonerne er udgivet under [CC BY 3.0](https://creativecommons.org/licenses/by/3.0/).
De må bruges frit, også ændret, men ophavspersonen skal krediteres.

| Fil | Ikon | Lavet af |
| --- | --- | --- |
| | | |
