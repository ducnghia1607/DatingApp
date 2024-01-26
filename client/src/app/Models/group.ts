export class Group {
  name?: string;
  connections: Connection[] = [];
}

export class Connection {
  connectionId?: string;
  username?: string;
}
