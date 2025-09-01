export class apiNews_Translate{
    from: string;
    to: string;
    title: string;
    content: string;

    public constructor(from: string, to: string, title: string, content: string)
    {
        this.from = from;
        this.to = to;
        this.title = title;
        this.content = content;
    }
}