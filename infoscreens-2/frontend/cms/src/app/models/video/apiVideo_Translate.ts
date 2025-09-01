export class apiVideo_Translate{
    from: string;
    to: string;
    title: string;

    public constructor(from: string, to: string, title: string)
    {
        this.from = from;
        this.to = to;
        this.title = title;
    }
}