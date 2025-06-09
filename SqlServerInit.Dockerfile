FROM mcr.microsoft.com/mssql-tools
COPY ./ERD_DATN.sql /ERD_DATN.sql
COPY ./init-db.sh /init-db.sh
RUN chmod +x /init-db.sh
CMD ["bash", "/init-db.sh"]