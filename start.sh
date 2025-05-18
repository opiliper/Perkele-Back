gnome-terminal -- bash -c "dotnet watch --project ./apps/Board/Board.csproj; exec bash"
gnome-terminal -- bash -c "dotnet watch --project ./apps/Gateway/Gateway.csproj; exec bash"
gnome-terminal -- bash -c "dotnet watch --project ./apps/BoardUser/BoardUser.csproj; exec bash"
gnome-terminal -- bash -c "dotnet watch --project ./apps/Users/Users.csproj; exec bash"